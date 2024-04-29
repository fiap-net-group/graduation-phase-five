using Mapster;
using TechBlog.Application.News.Create.Boundaries;
using TechBlog.Domain.Entities;

namespace TechBlog.Application.News.Update.Boundaries
{
    public class UpdateNewMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BlogNewEntity, UpdateNewOutput>();
        }
    }
}
