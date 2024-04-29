using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TechBlog.Application.News.GetByStrategy.Boundaries;
using TechBlog.Application.News.GetByStrategy.Strategies;
using TechBlog.Common.Responses;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Application.News.GetByStrategy
{
    public sealed class GetNewsByStrategyInteractor : IGetNewsByStrategyUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IValidator<GetNewsByStrategyInput> _validator;
        private readonly List<IGetNewsStrategy> _strategies;

        public GetNewsByStrategyInteractor(
            ILoggerManager logger, 
            IValidator<GetNewsByStrategyInput> validator, 
            IEnumerable<IGetNewsStrategy> strategies)
        {
            _logger = logger;
            _validator = validator;
            _strategies = strategies.ToList();
        }

        public async Task<GetNewsByStrategyOutput> GetAsync(GetNewsByStrategyInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Begin get news", LoggerManagerSeverity.DEBUG, ("strategy", input.Strategy));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);
            
            _logger.Log("Searching strategy", LoggerManagerSeverity.DEBUG, ("strategy", Enum.GetName(input.Strategy)));

            var matchedStrategies = _strategies.Where(s => s.Strategy == input.Strategy).ToList();

            if (matchedStrategies.Count == 0)
            {
                _logger.LogException(ResponseMessage.StrategyIsNotImplemented.ToString(), LoggerManagerSeverity.CRITICAL, parameters: ("strategy", Enum.GetName(input.Strategy)));

                throw new NotImplementedException(ResponseMessage.StrategyIsNotImplemented.ToString());
            }

            if (matchedStrategies.Count > 1)
            {
                _logger.LogException(ResponseMessage.StrategyHasMoreThanOneImplementation.ToString(), LoggerManagerSeverity.CRITICAL, parameters: ("strategy", Enum.GetName(input.Strategy)));

                throw new ArgumentException(ResponseMessage.StrategyHasMoreThanOneImplementation.ToString());
            }

            _logger.Log("Strategy was found", LoggerManagerSeverity.DEBUG, ("strategy", Enum.GetName(input.Strategy)));

            var output = await matchedStrategies[0].GetAsync(input, cancellationToken);

            _logger.Log("End get news", LoggerManagerSeverity.DEBUG, ("strategy", input.Strategy));

            return output;
        }
    }
}
