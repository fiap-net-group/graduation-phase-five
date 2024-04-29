using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Authentication.Login;
using TechBlog.Common.Api.Controllers;
using TechBlog.Application.Authentication.Login.Boundaries;
using Swashbuckle.AspNetCore.Annotations;
using TechBlog.Common.Responses;
using TechBlog.Application.Authentication.RefreshToken;
using Microsoft.AspNetCore.Authorization;
using TechBlog.Application.Authentication.RefreshToken.Boundaries;
using TechBlog.Domain.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace TechBlog.Users.API.Controllers.V1
{
    [ApiVersion("1")]
    [ExcludeFromCodeCoverage]
    [SwaggerTag("Endpoints for authentication and authorization management")]
    public sealed class AuthenticationController : ApiController
    {
        private readonly ILoginUseCase _loginUseCase;
        private readonly IRefreshTokenUseCase _refreshTokenUseCase;

        public AuthenticationController(
            IConfiguration configuration,
            ILoginUseCase loginUseCase,
            IRefreshTokenUseCase refreshTokenUseCase) : base(configuration)
        {
            _loginUseCase = loginUseCase;
            _refreshTokenUseCase = refreshTokenUseCase;
        }

        [HttpPost("login")]
        [SwaggerOperation(
            Summary = "Generate a authentication token after login", 
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/49/POST-Login'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Valid credentials", Type = typeof(LoginOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid credentials", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PostLogin([SwaggerRequestBody("User credentials")] LoginInput input, CancellationToken cancellationToken)
        {
            var output = await _loginUseCase.LoginAsync(input, AsCombinedCancellationToken(cancellationToken));

            return Ok(output);
        }

        [Authorize]
        [HttpPost("refresh-token")]
        [SwaggerOperation(
            Summary = "Generate a new authentication token by refresh token", 
            Description = "For more information, access our <a href='https://dev.azure.com/fiap-net-group/FIAP/_wiki/wikis/FIAP.wiki/61/POST-Refresh-Token'>wiki</a>.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Valid refresh token", Type = typeof(RefreshTokenOutput))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid refresh token", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, Description = "Invalid access token or access key", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Server error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PostRefreshToken([SwaggerRequestBody("The refresh token")] RefreshTokenInput input, CancellationToken cancellationToken)
        {
            input.Id = HttpContext.User.GetUserId();

            var output = await _refreshTokenUseCase.RefreshTokenAsync(input, AsCombinedCancellationToken(cancellationToken));

            return Ok(output);
        }
    }
}
