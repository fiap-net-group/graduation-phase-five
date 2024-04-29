using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using TechBlog.Domain.Gateways.Integrations.UsersApi;
using TechBlog.Domain.Gateways.Logger;

namespace TechBlog.Infrastructure.Integrations
{
    public sealed class UsersApi : IUsersApi
    {
        private readonly ILoggerManager _logger;
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly string _apiKey;
        private readonly string _getUserV1Endpoint;

        public UsersApi(
            ILoggerManager logger,
            HttpClient client,
            IConfiguration configuration,
            IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _client = client;
            _contextAccessor = contextAccessor;

            _apiKey = configuration.GetValue("Gateways:Integrations:UserApi:ApiKey", "");
            _getUserV1Endpoint = configuration.GetValue("Gateways:Integrations:UserApi:Endpoints:V1:GetUser", "");
        }

        public async Task<bool> ExistsByIdAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Log("Validating if user exists on Users API", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.Id, id));

                _client.DefaultRequestHeaders.Add("X-API-KEY", _apiKey);
                _client.DefaultRequestHeaders.Add(HeaderNames.Authorization, _contextAccessor.HttpContext.Request.Headers.Authorization.ToString());

                var endpoint = string.Format(_getUserV1Endpoint, id);

                var response = await _client.GetAsync(endpoint, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if(!response.IsSuccessStatusCode)
                {
                    _logger.Log("User don't exists on Users API", LoggerManagerSeverity.DEBUG,
                        (LoggingConstants.Id, id),
                        (LoggingConstants.Response, responseContent));

                    return false;
                }

                _logger.Log("User exists on Users API", LoggerManagerSeverity.DEBUG,
                    (LoggingConstants.Id, id),
                    (LoggingConstants.Response, responseContent));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogException("An error occurred while validating if user exists on Users API", LoggerManagerSeverity.WARNING, ex,
                    (LoggingConstants.Id, id));

                return false;
            }
        }
    }
}
