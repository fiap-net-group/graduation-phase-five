using TechBlog.Application.User.GetById.Boundaries;

namespace TechBlog.Application.User.GetById
{
    public interface IGetUserByIdUseCase
    {
        Task<GetUserByIdOutput> GetByIdAsync(GetUserByIdInput input, CancellationToken cancellationToken);
    }
}
