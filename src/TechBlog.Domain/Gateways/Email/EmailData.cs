namespace TechBlog.Domain.Gateways.Email
{
    public sealed class EmailData
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
    }
}
