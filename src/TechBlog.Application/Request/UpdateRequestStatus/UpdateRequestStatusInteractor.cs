using FluentValidation;
using TechBlog.Domain.Gateways.Event;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Application.Request.UpdateRequestStatus.Boundaries;
using TechBlog.Domain.Gateways.MemoryCache;

namespace TechBlog.Application.Request.UpdateRequestStatus;

public sealed class UpdateRequestStatusInteractor : IUpdateRequestStatusUseCase
{
    private readonly ILoggerManager _logger;
    private readonly IValidator<UpdateRequestStatusInput> _validator;
    private readonly IMemoryCacheManager _memoryCache;

    public UpdateRequestStatusInteractor(ILoggerManager logger, IEventSenderManager eventManager, IValidator<UpdateRequestStatusInput> validator, IMemoryCacheManager memoryCache)
    {
        _logger = logger;
        _validator = validator;
        _memoryCache = memoryCache;
    }

    public async Task UpdateAsync(UpdateRequestStatusInput request, CancellationToken cancellationToken)
    {
        _logger.Log("Starting updating the request status", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, request));

        var validation = await _validator.ValidateAsync(request, cancellationToken);

        if (!validation.IsValid)
        {
            _logger.Log("Request is not valid", LoggerManagerSeverity.WARNING,
                    (LoggingConstants.RequestEntity, request),
                    (LoggingConstants.Validation, validation));

            return;
        }

        _logger.Log("Request is valid", LoggerManagerSeverity.DEBUG, (LoggingConstants.RequestEntity, request));

        _logger.Log("Updating the request status on memory cache", LoggerManagerSeverity.DEBUG, (LoggingConstants.RequestEntity, request));
        await _memoryCache.CreateOrUpdate(Guid.Parse(request.Value.Id), request.Value, cancellationToken);
        _logger.Log("Request status updated on memory cache", LoggerManagerSeverity.DEBUG, (LoggingConstants.RequestEntity, request));

        _logger.Log("Ending updating the request status", LoggerManagerSeverity.INFORMATION, (LoggingConstants.Input, request));
    }
}
