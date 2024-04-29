using FluentValidation;
using Mapster;
using TechBlog.Application.User.Update.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.User.Update
{
    public sealed class UpdateUserInteractor : IUpdateUserUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<UpdateUserInput> _validator;
        private readonly IIdentityManager _identityManager;

        public UpdateUserInteractor(
            ILoggerManager logger, 
            IValidator<UpdateUserInput> validator, 
            IIdentityManager identityManager)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
        }

        public async Task<UpdateUserOutput> UpdateAsync(UpdateUserInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting updating the user", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var existingUser = await _identityManager.GetByIdAsync(input.Id, cancellationToken);

            if (!existingUser.IsActive)
                throw new NotFoundException();

            if(!string.IsNullOrWhiteSpace(input.Email))
            {
                var userWithSameEmail = await _identityManager.GetByEmailAsync(input.Email, cancellationToken);

                if (userWithSameEmail.Exists)
                    throw new BusinessException(ResponseMessage.InvalidEmail.ToString());
            }

            if (!existingUser.Update(input.Name, input.Email))
                throw new BusinessException(ResponseMessage.InvalidBody.ToString());

            if (!await _identityManager.UpdateUserAsync(existingUser.Id, existingUser, cancellationToken))
                throw new InfrastructureException("Error updating user");

            _logger.Log("Success updating user", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Id, input.Id));

            return existingUser.Adapt<UpdateUserOutput>();
        }
    }
}
