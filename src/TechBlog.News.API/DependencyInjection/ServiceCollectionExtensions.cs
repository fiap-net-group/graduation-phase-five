using TechBlog.Application;
using TechBlog.Infrastructure;
using TechBlog.Common;

namespace TechBlog.News.API.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiDependencyInjection()
                    .AddNewsApplicationConfiguration()
                    .AddLoggingManager(configuration)
                    .AddUsersApiIntegration(configuration)
                    .AddDatabase(configuration)
                    .AddIdentityClient(configuration)
                    .AddSwaggerDependencyInjection();

            return services;
        }
    }
}
