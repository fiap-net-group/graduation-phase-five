using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TechBlog.Application.Email.Send.Boundaries;
using TechBlog.Application.Request.UpdateRequestStatus;
using TechBlog.Application.Request.UpdateRequestStatus.Boundaries;
using TechBlog.Domain.Gateways.Email;
using TechBlog.Domain.Gateways.Logger;
using TechBlog.Domain.Gateways.MemoryCache;
using TechBlog.Infrastructure.Email.Boundaries;
using TechBlog.Infrastructure.Email;
using TechBlog.Infrastructure.Logger;
using TechBlog.Infrastructure.MemoryCache;
using Microsoft.Extensions.Caching.Memory;
using TechBlog.Infrastructure.Event;
using TechBlog.Domain.Gateways.Event;
using TechBlog.Domain.Extensions;
using TechBlog.Domain.Gateways.Integrations.UsersApi;
using TechBlog.Infrastructure.Integrations;
using TechBlog.Domain.Gateways.Database;
using TechBlog.Infrastructure.Database.Repositories;
using TechBlog.Domain.Gateways.Identity;
using TechBlog.Domain.ValueObjects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechBlog.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using TechBlog.Infrastructure.Logger.Boundaries;
using TechBlog.Infrastructure.Identity.Configuration;
using TechBlog.Infrastructure.Database.Configuration;
using TechBlog.Infrastructure.Database;
using TechBlog.Domain.Gateways.Integrations.NotificationsApi;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TechBlog.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public static class InfrastructureDependencyModule
    {
        public static IServiceCollection AddUsersApiIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient<IUsersApi, UsersApi>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetValue("Gateways:Integrations:UserApi:BaseAddress", ""));
                client.Timeout = new TimeSpan(0, 0, configuration.GetValue("Gateways:Integrations:UserApi:TimeoutInSeconds", 30));
            });

            return services;
        }

        public static IServiceCollection AddNotificationsApiIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<INotificationsApi, NotificationsApi>(client =>
            {
                client.BaseAddress = new Uri(configuration.GetValue("Gateways:Integrations:NotificationsApi:BaseAddress", ""));
                client.Timeout = new TimeSpan(0, 0, configuration.GetValue("Gateways:Integrations:NotificationsApi:TimeoutInSeconds", 30));
            });

            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IBlogNewsRepository, BlogNewsRepository>();

            services.AddSingleton<IMongoDbGateway, MongoDbGateway>();

            return services;
        }

        public static IServiceCollection AddMemoryCacheManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddScoped<IMemoryCacheManager, MicrosoftMemoryManager>();
            services.AddSingleton(new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(configuration.GetValue("Gateways:MemoryCache:SlidingExpirationInMinutes", 2))
            });

            return services;
        }

        public static IServiceCollection AddEmailManager(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailManager, SmtpEmailManager>();

            services.AddSingleton(provider => new EmailConfiguration(
                 From: configuration.GetValue("Gateways:Email:From", "graduationphasefour@outlook.com"),
                 DisplayName: configuration.GetValue("Gateways:Email:DisplayName", "Graduation Phase Four"),
                 Password: configuration.GetValue("Gateways:Email:Password", "Fiap@123"),
                 Host: configuration.GetValue("Gateways:Email:Host", "smtp.office365.com"),
                 Port: configuration.GetValue("Gateways:Email:Port", 587)));

            return services;
        }

        public static IServiceCollection AddLoggingManager(this IServiceCollection services, IConfiguration configuration, bool isWorker = false)
        {
            if (!configuration.IsProduction())
            {
                services.AddSingleton<ILoggerManager, ConsoleLoggerManager>();
                return services;
            }

            services.AddSingleton<ILoggerManager, ApplicationInsightsLoggerManager>();

            if (isWorker)
                services.AddWorkerLogging(configuration);
            else
                services.AddApiLogging(configuration);

            var telemetryConfiguration = new TelemetryConfiguration
            {
                ConnectionString = configuration.GetValue("Logging:Configuration:ConnectionString", "")
            };

            services.AddSingleton<ITelemetryInitializer, ApplicationInsightsCloudRoleNameInitializer>();
            services.AddSingleton(new TelemetryClient(telemetryConfiguration));

            return services;
        }

        private static IServiceCollection AddWorkerLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var loggingOptions = new Microsoft.ApplicationInsights.WorkerService.ApplicationInsightsServiceOptions
            {
                ConnectionString = configuration.GetValue("Logging:Configuration:ConnectionString", ""),
                DeveloperMode = false,
                AddAutoCollectedMetricExtractor = false
            };

            services.AddApplicationInsightsTelemetryWorkerService(options: loggingOptions);

            return services;
        }

        private static IServiceCollection AddApiLogging(this IServiceCollection services, IConfiguration configuration)
        {
            var loggingOptions = new ApplicationInsightsServiceOptions
            {
                ConnectionString = configuration.GetValue("Logging:Configuration:ConnectionString", ""),
                EnableRequestTrackingTelemetryModule = true,
                DeveloperMode = false,
                AddAutoCollectedMetricExtractor = false
            };

            loggingOptions.DependencyCollectionOptions.EnableLegacyCorrelationHeadersInjection = true;
            loggingOptions.RequestCollectionOptions.TrackExceptions = true;

            services.AddApplicationInsightsTelemetry(loggingOptions);

            return services;
        }

        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IIdentityManager, EntityFrameworkIdentityManager>();

            var connectionString = configuration.GetValue("Gateways:Identity:ConnectionString", "");
            services.AddDbContext<EntityFrameworkIdentityContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddIdentity<EntityFrameworkIdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            }
            ).AddEntityFrameworkStores<EntityFrameworkIdentityContext>()
             .AddDefaultTokenProviders();

            services.AddIdentityClient(configuration);

            return services;
        }

        public static IApplicationBuilder UseIdentityServer(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            using var identityContext = serviceScope.ServiceProvider.GetRequiredService<EntityFrameworkIdentityContext>();

            identityContext.MigrateAsync().Wait();
            identityContext.TestConnectionAsync().Wait();

            return app;
        }

        public static IServiceCollection AddIdentityClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["Gateways:Identity:Jwt:Issuer"],
                    ValidAudience = configuration["Gateways:Identity:Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Gateways:Identity:Jwt:Key"]))
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicies.IsJournalist, policy => policy.RequireClaim("BlogUserType", Enum.GetName(BlogUserType.JOURNALIST)));
            });

            return services;
        }

        public static IServiceCollection AddNotificationApiEventManager(this IServiceCollection services, IConfiguration configuration)
            => services.AddEventManager<UpdateRequestStatusService>(configuration, typeof(UpdateRequestStatusService), nameof(UpdateRequestStatusEvent), false);

        public static IServiceCollection AddEventConsumerManager<TWorker>(this IServiceCollection services, IConfiguration configuration, Type workerType, string queueName)
            where TWorker : class, IConsumer
            => services.AddEventManager<TWorker>(configuration, workerType, queueName, true);

        private static IServiceCollection AddEventManager<TWorker>(this IServiceCollection services, IConfiguration configuration, Type workerType, string queueName, bool isWorker)
            where TWorker : class, IConsumer
        {
            services.AddScoped<IEventSenderManager, MassTransitSenderManager>();

            services.AddMassTransit(busConfiguration =>
            {
                busConfiguration.AddBusConfiguration<TWorker>(configuration, queueName, isWorker);

                busConfiguration.AddConsumer(workerType);
            });

            return services;
        }

        private static IBusRegistrationConfigurator AddBusConfiguration<TWorker>(this IBusRegistrationConfigurator busConfiguration, IConfiguration configuration, string queueName, bool isWorker)
            where TWorker : class, IConsumer
        {
            if (configuration.IsProduction())
            {
                busConfiguration.UsingAzureServiceBus((context, cfg) =>
                {
                    var conn = configuration.GetValue<string>("Gateways:Event:ServiceBus:ConnectionString", "nao achou");
                    Console.WriteLine($"Conexão: {conn}");
                    cfg.Host(configuration.GetValueOrThrow<string>("Gateways:Event:ServiceBus:ConnectionString"), h => { });

                    if (!isWorker)
                        cfg.AddMessagesTypes();

                    cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.Consumer<TWorker>(context);
                    });
                });
            }
            else
            {
                busConfiguration.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetValueOrThrow<string>("Gateways:Event:RabbitMq:Server"), "/", h =>
                    {
                        h.Username(configuration.GetValueOrThrow<string>("Gateways:Event:RabbitMq:Username"));
                        h.Password(configuration.GetValueOrThrow<string>("Gateways:Event:RabbitMq:Password"));
                    });

                    cfg.ReceiveEndpoint(queueName, e =>
                    {
                        e.Consumer<TWorker>(context);
                    });
                });
            }

            return busConfiguration;
        }

        private static IServiceBusBusFactoryConfigurator AddMessagesTypes(this IServiceBusBusFactoryConfigurator cfg)
        {
            //Insert all messages types that are sent by the API
            cfg.Message<SendEmailEvent>(m => m.SetEntityName(nameof(SendEmailEvent).ToLower()));

            return cfg;
        }
    }
}