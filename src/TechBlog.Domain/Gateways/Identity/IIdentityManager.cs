using TechBlog.Domain.Entities;

namespace TechBlog.Domain.Gateways.Identity
{
    public interface IIdentityManager
    {
        Task<BlogUserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<BlogUserEntity> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<bool> CreateUserAsync(BlogUserEntity user, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
        Task<bool> ReactivateAsync(BlogUserEntity user, CancellationToken cancellationToken);
        Task<bool> ChangePasswordAsync(string id, string currentPassword, string newPassword, CancellationToken cancellationToken);
        Task<AccessTokenModel> AuthenticateAsync(BlogUserEntity user, string password, CancellationToken cancellationToken, params (string name, string value)[] customClaims);
        Task<AccessTokenModel> RefreshTokenAsync(string id, string refreshToken, CancellationToken cancellationToken, params (string name, string value)[] customClaims);
        Task<bool> UpdateUserAsync(string id, BlogUserEntity existingUser, CancellationToken cancellationToken);
    }
}
