using FluentValidation;
using TechBlog.Application.User.Delete.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;

namespace TechBlog.Application.User.Delete
{
    public sealed class DeleteUserInteractor : IDeleteUserUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<DeleteUserInput> _validator;
        private readonly IIdentityManager _identityManager;

        public DeleteUserInteractor(
            ILoggerManager logger, 
            IValidator<DeleteUserInput> validator, 
            IIdentityManager identityManager)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
        }

        public async Task DeleteAsync(DeleteUserInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting deleting the user", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var existingUser = await _identityManager.GetByIdAsync(input.Id, cancellationToken);

            if (!existingUser.IsActive)
                throw new NotFoundException();

            if (!await _identityManager.DeleteAsync(existingUser.Id, cancellationToken))
                throw new InfrastructureException("Error deleting user");

            _logger.Log("Success deleting user", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Id, input.Id));
        }
    }
}
