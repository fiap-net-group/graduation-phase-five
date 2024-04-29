using TechBlog.Domain.Gateways.Event;
using TechBlog.Application.Request.GetRequest.Boundaries;

namespace TechBlog.Application.Request.GetRequest
{
    public interface IGetRequestUseCase
    {
        Task<GetRequestOutput> GetAsync(GetRequestInput input, CancellationToken cancellationToken);
    }
}
