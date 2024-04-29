using TechBlog.Application.News.Update.Boundaries;

namespace TechBlog.Application.News.Update
{
    public interface IUpdateNewUseCase
    {
        Task<UpdateNewOutput> UpdateAsync(UpdateNewInput input, CancellationToken cancellationToken);
    }
}
