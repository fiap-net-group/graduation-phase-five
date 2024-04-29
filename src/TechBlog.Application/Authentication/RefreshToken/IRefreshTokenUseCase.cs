using TechBlog.Application.Authentication.RefreshToken.Boundaries;

namespace TechBlog.Application.Authentication.RefreshToken
{
    public interface IRefreshTokenUseCase
    {
        Task<RefreshTokenOutput> RefreshTokenAsync(RefreshTokenInput input, CancellationToken cancellationToken);
    }
}
