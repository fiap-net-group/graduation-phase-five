using FluentValidation;
using TechBlog.Domain.Entities;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Event;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.Gateways.MemoryCache;
using Mapster;
using TechBlog.Application.Request.CreateRequest.Boundaries;

namespace TechBlog.Application.Request.CreateRequest
{
    public sealed class CreateRequestInteractor : ICreateRequestUseCase
    {
        private readonly ILoggerManager _logger;
        private readonly IEventSenderManager _eventManager;
        private readonly IValidator<CreateRequestInput> _validator;
        private readonly IMemoryCacheManager _memoryCache;

        public CreateRequestInteractor(
            ILoggerManager logger,
            IEventSenderManager eventManager,
            IValidator<CreateRequestInput> validator, IMemoryCacheManager memoryCache)
        {
            _logger = logger;
            _eventManager = eventManager;
            _validator = validator;
            _memoryCache = memoryCache;
        }

        public async Task<CreateRequestOutput> CreateAsync<TEvent, TEventBody>(CreateRequestInput input, CancellationToken cancellationToken)
            where TEvent : BaseEvent<TEventBody>
        {
            _logger.Log("Starting creating the request", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, input));

            await _validator.ThrowIfInvalidAsync(input, _logger, cancellationToken);

            var requestId = await _eventManager.SendAsync<TEvent, TEventBody>(input.Adapt<TEvent>(), cancellationToken);

            var requestEntity = new RequestEntity
            {
                Id = requestId.ToString(),
                Status = Domain.ValueObjects.RequestStatus.NotStarted,
                Message = Domain.ValueObjects.ResponseMessage.Default
            };

            await _memoryCache.CreateOrUpdate(requestId, requestEntity, cancellationToken);

            _logger.Log("Ending creating the request", LoggerManagerSeverity.INFORMATION, (LoggingConstants.RequestEntity, requestEntity));

            return new CreateRequestOutput(requestId);
        }
    }
}