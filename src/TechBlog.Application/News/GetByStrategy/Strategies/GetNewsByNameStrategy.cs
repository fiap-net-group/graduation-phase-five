﻿using Mapster;
using TechBlog.Application.Common.Boundaries;
using TechBlog.Application.News.GetByStrategy.Boundaries;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.GetByStrategy.Strategies
{
    public sealed class GetNewsByNameStrategy : IGetNewsStrategy
    {
        private readonly ILoggerManager _logger;
        private readonly IBlogNewsRepository _repository;

        public GetNewsByNameStrategy(
            ILoggerManager logger,
            IBlogNewsRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public GetNewsStrategy Strategy => GetNewsStrategy.GetByName;

        public async Task<GetNewsByStrategyOutput> GetAsync(GetNewsByStrategyInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Getting blognew by name", LoggerManagerSeverity.DEBUG,
                ("strategy", Strategy),
                ("input", input));

            if (input is null || !input.ValidName())
            {
                _logger.Log("Invalid body", LoggerManagerSeverity.INFORMATION,
                    ("strategy", Strategy),
                    ("input", input));

                throw new BusinessException(ResponseMessage.InvalidBody.ToString());
            }

            var blogNews = (await _repository.GetByNameAsync(input.Name, cancellationToken)).ToList();

            _logger.Log("End getting blognew by name", LoggerManagerSeverity.DEBUG,
                ("strategy", Strategy),
                ("input", input),
                ("newsFoundCount", blogNews.Count));

            return new GetNewsByStrategyOutput(blogNews.Select(blogNew => blogNew.Adapt<BlogNewPort>()).ToList());
        }
    }
}
