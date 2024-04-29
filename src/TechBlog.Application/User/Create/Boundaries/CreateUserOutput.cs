using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.User.Create.Boundaries
{
    public sealed class CreateUserOutput : AccessTokenModel
    {
        public CreateUserOutput(AccessTokenModel accessToken) : base(accessToken.TokenType, accessToken.AccessToken, accessToken.Expires, accessToken.RefreshToken, accessToken.RefreshExpires, accessToken.UserId) { }
    }
}
