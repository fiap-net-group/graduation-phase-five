using TechBlog.Application.User.Delete.Boundaries;

namespace TechBlog.Application.User.Delete
{
    public interface IDeleteUserUseCase
    {
        Task DeleteAsync(DeleteUserInput input, CancellationToken cancellationToken);
    }
}
