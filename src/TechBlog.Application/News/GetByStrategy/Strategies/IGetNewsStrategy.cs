using TechBlog.Application.News.GetByStrategy.Boundaries;

namespace TechBlog.Application.News.GetByStrategy.Strategies
{
    public interface IGetNewsStrategy
    {
        GetNewsStrategy Strategy { get; }
        Task<GetNewsByStrategyOutput> GetAsync(GetNewsByStrategyInput input, CancellationToken cancellationToken);
    }
}
