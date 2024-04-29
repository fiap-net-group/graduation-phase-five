using TechBlog.Common;

namespace TechBlog.Notification.API;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDependencyInjection(this WebApplication app, IConfiguration configuration)
        {
            app.UseApiDependencyInjection(configuration, false);

            return app;
        }
    }
