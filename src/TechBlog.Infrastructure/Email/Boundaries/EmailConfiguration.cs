namespace TechBlog.Infrastructure.Email.Boundaries
{
    public sealed record EmailConfiguration(string From, string DisplayName, string Password, string Host, int Port);
}

