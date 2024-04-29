using TechBlog.Application.User.Update.Boundaries;

namespace TechBlog.Application.User.Update
{
    public interface IUpdateUserUseCase
    {
        Task<UpdateUserOutput> UpdateAsync(UpdateUserInput input, CancellationToken cancellationToken);
    }
}
