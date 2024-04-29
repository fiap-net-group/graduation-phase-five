using FluentValidation;
using Mapster;
using TechBlog.Application.User.Delete.Boundaries;
using TechBlog.Application.User.GetById.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;

namespace TechBlog.Application.User.GetById
{
    public class GetUserByIdInteractor : IGetUserByIdUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<GetUserByIdInput> _validator;
        private readonly IIdentityManager _identityManager;

        public GetUserByIdInteractor(
            ILoggerManager logger, 
            IValidator<GetUserByIdInput> validator, 
            IIdentityManager identityManager)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
        }

        public async Task<GetUserByIdOutput> GetByIdAsync(GetUserByIdInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting getting the user by id", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var existingUser = await _identityManager.GetByIdAsync(input.Id, cancellationToken);

            if (!existingUser.IsActive)
                throw new NotFoundException();

            _logger.Log("Success getting the user by id", LoggerManagerSeverity.DEBUG, (LoggingConstants.Id, input.Id));

            return existingUser.Adapt<GetUserByIdOutput>();
        }
    }
}
