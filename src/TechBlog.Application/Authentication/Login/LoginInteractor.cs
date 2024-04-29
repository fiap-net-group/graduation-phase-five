using FluentValidation;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TechBlog.Application.Authentication.Login.Boundaries;
using TechBlog.Domain.Gateways.Logger;
using static MassTransit.ValidationResultExtensions;
using TechBlog.Domain.ValueObjects;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Exceptions;
using Mapster;
using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.Authentication.Login
{
    public sealed class LoginInteractor : ILoginUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<LoginInput> _validator;
        private readonly IIdentityManager _identityManager;

        public LoginInteractor(
            ILoggerManager logger, 
            IValidator<LoginInput> validator,
            IIdentityManager identityManager)
        {
            _logger = logger;
            _validator = validator;
            _identityManager = identityManager;
        }

        public async Task<LoginOutput> LoginAsync(LoginInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Begin Login", LoggerManagerSeverity.DEBUG, (LoggingConstants.Username, input.Username));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var blogUser = await _identityManager.GetByEmailAsync(input.Username, cancellationToken);

            if (!blogUser.IsActive)
                throw new BusinessException(ResponseMessage.InvalidCredentials.ToString(), ResponseMessage.NotFound.ToString());
            
            var accessToken = await _identityManager.AuthenticateAsync(blogUser, input.Password, cancellationToken, (ClaimsExtensions.NameClaimIdentifier, blogUser.Name));

            if (!accessToken.Valid())
                throw new BusinessException(ResponseMessage.InvalidCredentials.ToString());

            _logger.Log("Success Login", LoggerManagerSeverity.DEBUG, (LoggingConstants.Username, input.Username));

            return accessToken.Adapt<LoginOutput>();
        }
    }
}
