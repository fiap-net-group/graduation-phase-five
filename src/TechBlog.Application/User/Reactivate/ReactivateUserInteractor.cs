using FluentValidation;
using Mapster;
using TechBlog.Application.User.Create.Boundaries;
using TechBlog.Application.User.Reactivate.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Reactivate
{
    public sealed class ReactivateUserInteractor : IReactivateUserUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<ReactivateUserInput> _validator;
        private readonly IIdentityManager _identityManager;

        public ReactivateUserInteractor(
            ILoggerManager logger, 
            IValidator<ReactivateUserInput> validator, 
            IIdentityManager identityManager)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
        }

        public async Task<ReactivateUserOutput> ReactivateAsync(ReactivateUserInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting reactivating the user", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input.WithoutPassword()));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var existingUser = await _identityManager.GetByIdAsync(input.Id, cancellationToken);

            if (!existingUser.Exists)
                throw new NotFoundException();

            if (existingUser.IsActive)
                throw new BusinessException(ResponseMessage.UserAlreadyExists.ToString());

            existingUser.Password = input.Password;

            if (!await _identityManager.ReactivateAsync(existingUser, cancellationToken))
                throw new InfrastructureException("Error reactivating user");

            var accessToken = await _identityManager.AuthenticateAsync(existingUser, input.Password, cancellationToken, (LoggingConstants.Name, existingUser.Name));

            _logger.Log("Success reactivating user", LoggerManagerSeverity.DEBUG, (LoggingConstants.Email, existingUser.Email));

            return accessToken.Adapt<ReactivateUserOutput>();
        }
    }
}
