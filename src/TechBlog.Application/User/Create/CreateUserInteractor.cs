using FluentValidation;
using Mapster;
using TechBlog.Application.User.Create.Boundaries;
using TechBlog.Application.User.Reactivate;
using TechBlog.Application.User.Reactivate.Boundaries;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Integrations.NotificationsApi;
using TechBlog.Domain.Gateways.Integrations.NotificationsApi.PostEmail;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Create
{
    public sealed class CreateUserInteractor : ICreateUserUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<CreateUserInput> _validator;
        private readonly IIdentityManager _identityManager;
        private readonly IReactivateUserUseCase _reactivateUserUseCase;
        private readonly INotificationsApi _notificationApi;

        public CreateUserInteractor(
            ILoggerManager logger,
            IValidator<CreateUserInput> validator,
            IIdentityManager identityManager,
            IReactivateUserUseCase reactivateUserUseCase,
            INotificationsApi notificationApi)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
            _reactivateUserUseCase = reactivateUserUseCase;
            _notificationApi = notificationApi;
        }

        public async Task<CreateUserOutput> CreateAsync(CreateUserInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting creating the user", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input.WithoutPassword()));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var existingUser = await _identityManager.GetByEmailAsync(input.Email, cancellationToken);

            if (existingUser.IsActive)
                throw new BusinessException(ResponseMessage.UserAlreadyExists.ToString());

            if(existingUser.Exists)
            {
                _logger.Log("User exists, but is disabled, reactivating", LoggerManagerSeverity.DEBUG, (LoggingConstants.Email, input.Email));

                existingUser.Password = input.Password;

                var reactivateUserOutput = await _reactivateUserUseCase.ReactivateAsync(existingUser.Adapt<ReactivateUserInput>(), cancellationToken);

                return reactivateUserOutput.Adapt<CreateUserOutput>();
            }

            _logger.Log("User don't exists, creting new", LoggerManagerSeverity.DEBUG, (LoggingConstants.Email, input.Email));

            var blogUser = input.Adapt<BlogUserEntity>();

            if (!await _identityManager.CreateUserAsync(blogUser, cancellationToken))
                throw new InfrastructureException(ResponseMessage.ErrorCreatingUser.ToString());

            var accessToken = await _identityManager.AuthenticateAsync(blogUser, input.Password, cancellationToken, (LoggingConstants.Name, blogUser.Name));

            await _notificationApi.SendEmailAsync(input.Adapt<PostEmailRequest>(), cancellationToken);

            _logger.Log("Success creating user", LoggerManagerSeverity.DEBUG, (LoggingConstants.Email, input.Email));

            return accessToken.Adapt<CreateUserOutput>();
        }
    }
}
