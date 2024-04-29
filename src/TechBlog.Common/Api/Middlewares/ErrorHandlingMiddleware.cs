using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using TechBlog.Common.Responses;
using TechBlog.Domain.Exceptions;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.ValueObjects;

namespace TechBlog.Common.Api.Middlewares
{
    public sealed class ErrorHandlingMiddleware
    {
        private readonly ILoggerManager _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(ILoggerManager logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    _logger.Log("Unauthorized error caught by middleware - JWT TOKEN", LoggerManagerSeverity.WARNING);

                    var code = HttpStatusCode.Unauthorized;

                    var result = JsonConvert.SerializeObject(new ErrorResponse(ResponseMessage.UserIsNotAuthenticated));

                    await ErrorResponse(context, result, code);
                }
            }
            catch (ForbiddenException ex)
            {
                _logger.Log("Forbidden error caught by middleware", LoggerManagerSeverity.WARNING, ("exception", ex));

                await HandleExceptionAsync(context, ex, HttpStatusCode.Forbidden);
            }
            catch(NotFoundException ex)
            {
                _logger.Log("Not found error caught by middleware", LoggerManagerSeverity.WARNING, ("exception", ex));

                await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
            }
            catch (BusinessException ex)
            {
                _logger.Log("Business error caught by middleware", LoggerManagerSeverity.WARNING, ("exception", ex));

                await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.Log("Unauthorized error caught by middleware - API TOKEN", LoggerManagerSeverity.WARNING, ("exception", ex));

                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _logger.LogException("Unexpected error caught by middleware", LoggerManagerSeverity.ERROR, ex);

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, BusinessException exception, HttpStatusCode code)
        {
            var result = JsonConvert.SerializeObject(new ErrorResponse(Enum.Parse<ResponseMessage>(exception.Message), exception.Errors));

            return ErrorResponse(context, result, code);
        }

        private static Task HandleExceptionAsync(HttpContext context, UnauthorizedAccessException exception)
        {
            var code = HttpStatusCode.Unauthorized;

            var result = JsonConvert.SerializeObject(new ErrorResponse(Enum.Parse<ResponseMessage>(exception.Message)));

            return ErrorResponse(context, result, code);
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(new ErrorResponse(ResponseMessage.UnexpectedError,
                $"message: {exception.Message}", $"inner: {exception.InnerException?.Message ?? "no inner"}", $"stackTrace: {exception.StackTrace}"));

            return ErrorResponse(context, result, code);
        }

        private static Task ErrorResponse(HttpContext context, string result, HttpStatusCode code)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}
