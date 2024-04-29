using TechBlog.Application.Request.UpdateRequestStatus.Boundaries;

namespace TechBlog.Application.Request.UpdateRequestStatus;

public interface IUpdateRequestStatusUseCase
{
    Task UpdateAsync(UpdateRequestStatusInput request, CancellationToken cancellationToken);
}
