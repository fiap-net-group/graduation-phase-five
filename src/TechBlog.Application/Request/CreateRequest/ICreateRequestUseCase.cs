using TechBlog.Domain.Gateways.Event;
using TechBlog.Application.Request.CreateRequest.Boundaries;

namespace TechBlog.Application.Request.CreateRequest
{
    public interface ICreateRequestUseCase
    {
        Task<CreateRequestOutput> CreateAsync<TEvent, TEventBody>(CreateRequestInput input, CancellationToken cancellationToken) where TEvent : BaseEvent<TEventBody>;
    }
}