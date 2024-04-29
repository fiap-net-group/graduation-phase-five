using TechBlog.Domain.Gateways.Email;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Infrastructure.Email.Boundaries;

namespace TechBlog.Infrastructure.Email
{
    public sealed class SmtpEmailManager : IEmailManager
    {
        private readonly EmailConfiguration _configuration;
        private readonly ILoggerManager _logger;

        public SmtpEmailManager(EmailConfiguration configuration, ILoggerManager logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendAsync(EmailData email, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Log("Sending email", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.Email, email));

                using var message = email.AsMailMessage(_configuration);

                using var smtp = _configuration.AsSmtpClient();

                await smtp.SendMailAsync(message, cancellationToken);

                _logger.Log("Email sent successfully", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.Email, email));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException("An error occurred sending the email", LoggerManagerSeverity.WARNING, ex,
                    (LoggingConstants.Email, email));

                return false;
            }
        }
    }
}
