namespace TechBlog.Domain.Gateways.Integrations.UsersApi
{
    public interface IUsersApi
    {
        Task<bool> ExistsByIdAsync(string id, CancellationToken cancellationToken);
    }
}
