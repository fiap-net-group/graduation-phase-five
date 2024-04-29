using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using TechBlog.Common.Api.Middlewares;
using TechBlog.Common.Api.Middlewares.Logger;
using TechBlog.Common.Api.Swagger;
using TechBlog.Domain.Extensions;

namespace TechBlog.Common
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiDependencyInjection(this WebApplication app, IConfiguration configuration, bool authenticationRequired)
        {
            app.UseRouting();

            if(authenticationRequired)
                app.UseAuthentication();


            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseMiddlewareIfProduction<LoggerMiddleware>(configuration.IsProduction());
            app.UseMiddlewareIfProduction<LogRequestMiddleware>(configuration.IsProduction());
            app.UseMiddlewareIfProduction<LogResponseMiddleware>(configuration.IsProduction());

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseMiddlewareIfProduction<ApiKeyMiddleware>(configuration.IsProduction());

            app.UseSwaggerConfiguration(configuration);

            app.UseAuthorization();

            app.UseCors(options => options.AllowAnyOrigin()
                                          .AllowAnyHeader()
                                          .AllowAnyMethod()) ;

            return app;
        }

        public static IApplicationBuilder UseMiddlewareIfProduction<T>(this WebApplication app, bool isProduction)
        {
            if (isProduction)
                app.UseMiddleware<T>();

            return app;
        }
    }
}
