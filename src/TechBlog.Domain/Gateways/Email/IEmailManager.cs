namespace TechBlog.Domain.Gateways.Email
{
    public interface IEmailManager
    {
        Task<bool> SendAsync(EmailData email, CancellationToken cancellationToken);
    }
}
