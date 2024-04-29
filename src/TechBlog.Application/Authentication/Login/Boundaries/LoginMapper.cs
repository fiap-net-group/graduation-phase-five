using Mapster;
using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.Authentication.Login.Boundaries
{
    public class LoginMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AccessTokenModel, LoginOutput>()
                .ConstructUsing(source => new LoginOutput(source));
        }
    }
}
