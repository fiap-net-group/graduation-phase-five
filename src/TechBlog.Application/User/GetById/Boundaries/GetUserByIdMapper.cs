using Mapster;
using TechBlog.Domain.Entities;

namespace TechBlog.Application.User.GetById.Boundaries
{
    public class GetUserByIdMapper : IRegister
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
