using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.Authentication.RefreshToken.Boundaries
{
    public sealed class RefreshTokenOutput : AccessTokenModel
    {
        public RefreshTokenOutput(AccessTokenModel accessToken) : base(accessToken.TokenType, accessToken.AccessToken, accessToken.Expires, accessToken.RefreshToken, accessToken.RefreshExpires, accessToken.UserId) { }
    }
}
