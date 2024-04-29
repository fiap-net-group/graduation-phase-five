using Mapster;
using TechBlog.Application.User.Create.Boundaries;
using TechBlog.Domain.Gateways.Identity;

namespace TechBlog.Application.User.Reactivate.Boundaries
{
    public class ReactivateUserMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AccessTokenModel, ReactivateUserOutput>()
                  .ConstructUsing(source => new ReactivateUserOutput(source))
                  ;
        }
    }
}
