using Mapster;
using TechBlog.Application.Common.Boundaries;
using TechBlog.Domain.Entities;

namespace TechBlog.Application.News.GetByStrategy.Boundaries
{
    public class GetNewsByStrategyMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<BlogNewEntity, BlogNewPort>()
                .ConstructUsing(entity => new BlogNewPort(entity));

            config.NewConfig<BlogNewEntity, GetNewsByStrategyOutput>()
                .ConstructUsing(entity => new GetNewsByStrategyOutput(new List<BlogNewPort>(1) { new(entity) }));
        }
    }
}
