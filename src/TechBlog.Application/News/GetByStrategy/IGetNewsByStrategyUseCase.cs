using TechBlog.Application.News.GetByStrategy.Boundaries;

namespace TechBlog.Application.News.GetByStrategy
{
    public interface IGetNewsByStrategyUseCase
    {
        Task<GetNewsByStrategyOutput> GetAsync(GetNewsByStrategyInput input, CancellationToken cancellationToken);   
    }
}
