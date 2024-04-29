using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Json;
using TechBlog.Domain.Gateways.Integrations.NotificationsApi;
using TechBlog.Domain.Gateways.Integrations.NotificationsApi.PostEmail;
using TechBlog.Domain.Gateways.Logger;

namespace TechBlog.Infrastructure.Integrations
{
    public sealed class NotificationsApi : INotificationsApi
    {
        private readonly ILoggerManager _logger;
        private readonly HttpClient _client;

        private readonly string _postV1Email;

        public NotificationsApi(
            ILoggerManager logger, 
            HttpClient client, 
            IConfiguration configuration)
        {
            _logger = logger;
            _client = client;

            _postV1Email = configuration.GetValue("Gateways:Integrations:NotificationsApi:Endpoints:V1:PostEmail", "");
        }

        public async Task SendEmailAsync(PostEmailRequest request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Log("Sending email", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.Email, request.To));

                var response = await _client.PostAsJsonAsync(_postV1Email, request, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.Log("Unable to send email", LoggerManagerSeverity.DEBUG,
                        (LoggingConstants.Email, request.To),
                        (LoggingConstants.Response, responseContent));

                    return;
                }

                _logger.Log("Email sent", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.Email, request.To),
                    (LoggingConstants.Response, responseContent));
            }
            catch (Exception ex)
            {
                _logger.LogException("An error occurred while sending email", LoggerManagerSeverity.WARNING, ex,
                    (LoggingConstants.Email, request.To));
            }
        }
    }
}
