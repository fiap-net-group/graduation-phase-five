using FluentValidation;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Application.Request.GetRequest.Boundaries;
using TechBlog.Domain.Gateways.MemoryCache;

namespace TechBlog.Application.Request.GetRequest
{
    public class GetRequestInteractor : IGetRequestUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IMemoryCacheManager _memoryCache;
        private readonly IValidator<GetRequestInput> _validator;

        public GetRequestInteractor(ILoggerManager logger, IMemoryCacheManager memoryCache, IValidator<GetRequestInput> validator)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _validator = validator;
        }
        public async Task<GetRequestOutput> GetAsync(GetRequestInput input, CancellationToken cancellationToken)
        {
            _logger.Log("Starting get request status", LoggerManagerSeverity.INFORMATION, ("request", input));

            _logger.Log("Validating the request", LoggerManagerSeverity.DEBUG, ("request", input));
            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);
            _logger.Log("Request is valid", LoggerManagerSeverity.DEBUG, ("request", input));

            var requestEntity = await _memoryCache.GetAsync<RequestEntity>(input.RequestId, cancellationToken);

            _logger.Log("Ending get request status", LoggerManagerSeverity.INFORMATION, ("request", input));

            return new GetRequestOutput(requestEntity);
        }
    }
}
