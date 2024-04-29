using TechBlog.Common;
using TechBlog.Domain.Extensions;

namespace TechBlog.News.API.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDependencyInjection(this WebApplication app, IConfiguration configuration)
        {
            app.UseApiDependencyInjection(configuration, true);

            return app;
        }
    }
}
