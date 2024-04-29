using TechBlog.Application.User.ChangePassword.Boundaries;

namespace TechBlog.Application.User.ChangePassword
{
    public interface IChangePasswordUseCase
    {
        Task<ChangePasswordOutput> ChangePasswordAsync(ChangePasswordInput input, CancellationToken cancellationToken);
    }
}
