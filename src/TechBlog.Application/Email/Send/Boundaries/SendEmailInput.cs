namespace TechBlog.Application.Email.Send.Boundaries
{
    public sealed class SendEmailInput
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public SendEmailInput() { }
        public SendEmailInput(string to, string subject, string body)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
    }
}
