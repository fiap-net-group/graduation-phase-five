using TechBlog.Domain.Gateways.Integrations.NotificationsApi.PostEmail;

namespace TechBlog.Domain.Gateways.Integrations.NotificationsApi
{
    public interface INotificationsApi
    {
        Task SendEmailAsync(PostEmailRequest request, CancellationToken cancellationToken);
    }
}
