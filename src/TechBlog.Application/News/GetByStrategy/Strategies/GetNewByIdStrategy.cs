using Mapster;
using TechBlog.Application.News.GetByStrategy.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.GetByStrategy.Strategies
{
    public sealed class GetNewByIdStrategy : IGetNewsStrategy
    {
        private readonly ILoggerManager _logger;
        private readonly IBlogNewsRepository _repository;

        public GetNewByIdStrategy(
            ILoggerManager logger,
            IBlogNewsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public GetNewsStrategy Strategy => GetNewsStrategy.GetById;

        public async Task<GetNewsByStrategyOutput> GetAsync(GetNewsByStrategyInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Getting blognew by id", LoggerManagerSeverity.DEBUG,
                ("strategy", Strategy),
                ("input", input));

            if (input is null || !input.ValidId())
            {
                _logger.Log("Invalid body", LoggerManagerSeverity.INFORMATION,
                    ("strategy", Strategy),
                    ("input", input));

                throw new BusinessException(ResponseMessage.InvalidBody.ToString());
            }

            var blogNew = await _repository.GetByIdAsync(input.Id, cancellationToken);

            if (!blogNew.Enabled)
                throw new NotFoundException();

            _logger.Log("End getting blognew by id", LoggerManagerSeverity.DEBUG,
                ("strategy", Strategy),
                ("input", input),
                ("blogNew", blogNew));

            return blogNew.Adapt<GetNewsByStrategyOutput>();
        }
    }
}
