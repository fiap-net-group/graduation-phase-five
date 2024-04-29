using TechBlog.Application.User.Reactivate.Boundaries;

namespace TechBlog.Application.User.Reactivate
{
    public interface IReactivateUserUseCase
    {
        Task<ReactivateUserOutput> ReactivateAsync(ReactivateUserInput input, CancellationToken cancellationToken);
    }
}
