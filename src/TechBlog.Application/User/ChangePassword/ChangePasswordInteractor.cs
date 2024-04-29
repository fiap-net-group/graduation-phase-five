using FluentValidation;
using Mapster;
using TechBlog.Application.User.ChangePassword.Boundaries;
using TechBlog.Application.User.Create.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;

namespace TechBlog.Application.User.ChangePassword
{
    public sealed class ChangePasswordInteractor : IChangePasswordUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<ChangePasswordInput> _validator;
        private readonly IIdentityManager _identityManager;

        public ChangePasswordInteractor(
            ILoggerManager logger, 
            IValidator<ChangePasswordInput> validator, 
            IIdentityManager identityManager)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
        }

        public async Task<ChangePasswordOutput> ChangePasswordAsync(ChangePasswordInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting changing the user password", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var existingUser = await _identityManager.GetByIdAsync(input.Id, cancellationToken);

            if (!existingUser.IsActive)
                throw new NotFoundException();

            if (!await _identityManager.ChangePasswordAsync(existingUser.Id, input.CurrentPassword, input.NewPassword, cancellationToken))
                throw new InfrastructureException("Error changing user password");

            _logger.Log("Success changing the user password", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Id, input.Id));

            return existingUser.Adapt<ChangePasswordOutput>();
        }
    }
}
