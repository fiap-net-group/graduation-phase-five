using FluentValidation;
using TechBlog.Application.News.GetByStrategy.Strategies;

namespace TechBlog.Application.News.GetByStrategy.Boundaries
{
    public sealed class GetNewsByStrategyValidator : AbstractValidator<GetNewsByStrategyInput>
    {
        public GetNewsByStrategyValidator()
        {
            
        }
    }
}
