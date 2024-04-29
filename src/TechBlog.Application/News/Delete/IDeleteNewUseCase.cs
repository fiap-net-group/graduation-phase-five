using TechBlog.Application.News.Delete.Boundaries;

namespace TechBlog.Application.News.Delete
{
    public interface IDeleteNewUseCase
    {
        Task<DeleteNewOutput> DeleteAsync(DeleteNewInput input, CancellationToken cancellationToken);
    }
}
