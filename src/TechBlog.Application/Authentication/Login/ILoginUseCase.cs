using TechBlog.Application.Authentication.Login.Boundaries;

namespace TechBlog.Application.Authentication.Login
{
    public interface ILoginUseCase
    {
        Task<LoginOutput> LoginAsync(LoginInput input, CancellationToken cancellationToken);
    }
}
