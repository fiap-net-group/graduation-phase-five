using TechBlog.Application.Common.Boundaries;

namespace TechBlog.Application.News.GetByStrategy.Boundaries
{
    public sealed record GetNewsByStrategyOutput(List<BlogNewPort> News);
}
