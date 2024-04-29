namespace TechBlog.Domain.Gateways.Event
{
    public interface IEventSenderManager
    {
        Task<Guid> SendAsync<TEvent, TEventBody>(TEvent genericEvent, CancellationToken cancellationToken) where TEvent : BaseEvent<TEventBody>;
    }
}
