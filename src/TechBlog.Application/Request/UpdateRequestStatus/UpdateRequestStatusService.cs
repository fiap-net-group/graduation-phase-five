using TechBlog.Domain.Gateways.Logger;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechBlog.Domain.Extensions;
using TechBlog.Application.Request.UpdateRequestStatus.Boundaries;

namespace TechBlog.Application.Request.UpdateRequestStatus;

public sealed class UpdateRequestStatusService : IConsumer<UpdateRequestStatusEvent>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerManager _logger;

    public UpdateRequestStatusService(IServiceProvider serviceProvider, ILoggerManager logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UpdateRequestStatusEvent> context)
    {
        using var scope = _serviceProvider.CreateScope();

        var updateRequestStatus = scope.ServiceProvider.GetRequiredService<IUpdateRequestStatusUseCase>();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var cancellationToken = configuration.GenerateCancellationToken();

        _logger.Log("Begin executing Update REquest Status business rule", LoggerManagerSeverity.DEBUG,
            (LoggingConstants.Event, context.Message));

        await updateRequestStatus.UpdateAsync(context.Message.Value, cancellationToken);

        _logger.Log("End executing Update REquest Status business rule", LoggerManagerSeverity.DEBUG,
                (LoggingConstants.Event, context.Message));
    }
}
