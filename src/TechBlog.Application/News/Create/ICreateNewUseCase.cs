using TechBlog.Application.News.Create.Boundaries;

namespace TechBlog.Application.News.Create
{
    public interface ICreateNewUseCase
    {
        Task<CreateNewOutput> CreateAsync(CreateNewInput input, CancellationToken cancellationToken);
    }
}
