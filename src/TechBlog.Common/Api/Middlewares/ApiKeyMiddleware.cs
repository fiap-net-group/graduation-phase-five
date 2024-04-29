using TechBlog.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TechBlog.Common.Responses;

namespace TechBlog.Common.Api.Middlewares
{
    public sealed class ApiKeyMiddleware
    {
        private readonly string _apiKey;
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(IConfiguration configuration, RequestDelegate next)
        {
            _apiKey = configuration["ApiKey"];
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.ToString().ToLower().Contains("swagger") &&
                context.Request.Headers["X-API-KEY"] != _apiKey)
            {
                throw new UnauthorizedAccessException(ResponseMessage.InvalidApiKey.ToString());
            }

            await _next(context);
        }
    }
}
