using TechBlog.Common;
using TechBlog.Domain.Extensions;
using TechBlog.Infrastructure;

namespace TechBlog.Users.API.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDependencyInjection(this WebApplication app, IConfiguration configuration)
        {
            app.UseApiDependencyInjection(configuration, true);

            app.UseIdentityServer();

            return app;
        }
    }
}
