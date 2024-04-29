using Mapster;
using TechBlog.Domain.Entities;

namespace TechBlog.Application.News.Create.Boundaries
{
    public class CreateNewMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateNewInput, BlogNewEntity>()
                  .Map(destination => destination.LastUpdateAt, source => DateTime.UtcNow);

            config.NewConfig<BlogNewEntity, CreateNewOutput>();
        }
    }
}
