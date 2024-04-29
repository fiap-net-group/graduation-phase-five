namespace TechBlog.Domain.Gateways.Integrations.NotificationsApi.PostEmail
{
    public sealed record PostEmailRequest(string To, string Subject, string Body);
}