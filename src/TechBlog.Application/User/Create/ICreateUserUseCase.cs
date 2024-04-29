using TechBlog.Application.User.Create.Boundaries;

namespace TechBlog.Application.User.Create
{
    public interface ICreateUserUseCase
    {
        Task<CreateUserOutput> CreateAsync(CreateUserInput input, CancellationToken cancellationToken);
    }
}
