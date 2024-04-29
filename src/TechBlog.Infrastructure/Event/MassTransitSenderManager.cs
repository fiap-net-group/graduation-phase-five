using TechBlog.Domain.Gateways.Event;
using TechBlog.Domain.Gateways.Logger;
using MassTransit;

namespace TechBlog.Infrastructure.Event
{
    public class MassTransitSenderManager : IEventSenderManager
    {
        private readonly IBus _bus;
        private readonly ILoggerManager _logger;

        public MassTransitSenderManager(IBus bus, ILoggerManager logger)
        {
            _bus = bus;
            _logger = logger;
        }

        public async Task<Guid> SendAsync<TEvent, TEventBody>(TEvent genericEvent, CancellationToken cancellationToken) where TEvent : BaseEvent<TEventBody>
        {
            _logger.Log("Sending event", LoggerManagerSeverity.DEBUG,
                (LoggingConstants.Event, genericEvent));

            if (genericEvent.RequestId == Guid.Empty)
                genericEvent.RequestId = Guid.NewGuid();

            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{genericEvent.OperationName}"));

            await endpoint.Send(genericEvent);

            _logger.Log("Event sent", LoggerManagerSeverity.DEBUG,
                (LoggingConstants.Event, genericEvent),
                (LoggingConstants.RequestId, genericEvent.RequestId));

            return genericEvent.RequestId;
        }
    }
}
