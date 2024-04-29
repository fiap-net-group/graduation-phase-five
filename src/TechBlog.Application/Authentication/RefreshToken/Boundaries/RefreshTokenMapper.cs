using Mapster;
using TechBlog.Application.Authentication.Login.Boundaries;
using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.Authentication.RefreshToken.Boundaries
{
    public class RefreshTokenMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AccessTokenModel, RefreshTokenOutput>()
                .ConstructUsing(source => new RefreshTokenOutput(source));
        }
    }
}
