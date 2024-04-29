using FluentValidation;
using Mapster;
using TechBlog.Application.Authentication.RefreshToken.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.Authentication.RefreshToken
{
    public sealed class RefreshTokenInteractor : IRefreshTokenUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<RefreshTokenInput> _validator;
        private readonly IIdentityManager _identityManager;

        public RefreshTokenInteractor(
            ILoggerManager logger, 
            IValidator<RefreshTokenInput> validator, 
            IIdentityManager identityManager)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
        }

        public async Task<RefreshTokenOutput> RefreshTokenAsync(RefreshTokenInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Begin refresh token", LoggerManagerSeverity.DEBUG, (LoggingConstants.Id, input.Id));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var accessToken = await _identityManager.RefreshTokenAsync(input.Id, input.RefreshToken, cancellationToken);

            if (!accessToken.Valid())
                throw new BusinessException(ResponseMessage.InvalidCredentials.ToString());

            _logger.Log("Success refreshing token", LoggerManagerSeverity.DEBUG, (LoggingConstants.Id, input.Id));

            return accessToken.Adapt<RefreshTokenOutput>();
        }
    }
}
