using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.User.Reactivate.Boundaries
{
    public sealed class ReactivateUserOutput : AccessTokenModel
    {
        public ReactivateUserOutput(AccessTokenModel accessToken) : base(accessToken.TokenType, accessToken.AccessToken, accessToken.Expires, accessToken.RefreshToken, accessToken.RefreshExpires, accessToken.UserId) { }
    }
}
