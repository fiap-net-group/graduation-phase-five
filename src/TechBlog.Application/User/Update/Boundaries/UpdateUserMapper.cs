using Mapster;
using TechBlog.Application.User.GetById.Boundaries;
using TechBlog.Domain.Entities;

namespace TechBlog.Application.User.Update.Boundaries
{
    public class UpdateUserMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BlogUserEntity, GetUserByIdOutput>()
                  .Map(destination => destination.Id, source => source.Id)
                  .Map(destination => destination.Name, source => source.Name)
                  .Map(destination => destination.BlogUserType, source => source.BlogUserType)
                  .Map(destination => destination.Email, source => source.Email)
                  ;
        }
    }
}
