using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.Authentication.Login.Boundaries
{
    public sealed class LoginOutput : AccessTokenModel
    {
        public LoginOutput(AccessTokenModel accessToken) : base(accessToken.TokenType, accessToken.AccessToken, accessToken.Expires, accessToken.RefreshToken, accessToken.RefreshExpires, accessToken.UserId) { }
    }
}
