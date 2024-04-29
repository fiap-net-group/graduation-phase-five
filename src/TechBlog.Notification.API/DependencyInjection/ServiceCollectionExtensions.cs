using TechBlog.Application;
using TechBlog.Infrastructure;
using TechBlog.Common;

namespace TechBlog.Notification.API;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiDependencyInjection()
                    .AddNotificationApplicationConfiguration()
                    .AddLoggingManager(configuration)
                    .AddLoggingManager(configuration)
                    .AddNotificationApiEventManager(configuration)
                    .AddMemoryCacheManager(configuration)
                    .AddSwaggerDependencyInjection();

        return services;
        }
    }
