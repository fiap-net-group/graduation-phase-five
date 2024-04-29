using TechBlog.Domain.Gateways.Email;
using System.Net;
using System.Net.Mail;

namespace TechBlog.Infrastructure.Email.Boundaries
{
    public static class EmailMapper
    {
        public static MailMessage AsMailMessage(this EmailData from, EmailConfiguration configuration) =>
            new(from: new MailAddress(configuration.From, configuration.DisplayName), to: new MailAddress(from.To))
            {
                Subject = from.Subject,
                Body = from.HtmlBody,
                IsBodyHtml = true
            };

        public static SmtpClient AsSmtpClient(this EmailConfiguration configuration) =>
            new()
            {
                Port = configuration.Port,
                Host = configuration.Host,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(configuration.From, configuration.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };
    }
}