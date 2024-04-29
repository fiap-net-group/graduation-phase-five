using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TechBlog.Application;
using TechBlog.Application.Email.Send;
using TechBlog.Application.Email.Send.Boundaries;
using TechBlog.Infrastructure;

IConfiguration configuration = default;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
    {
        configuration = new ConfigurationBuilder()
            .SetBasePath(hostBuilderContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        configurationBuilder.AddConfiguration(configuration);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSendEmailWorkerApplicationConfiguration();
        services.AddHttpContextAccessor();
        services.AddLoggingManager(configuration, true);
        services.AddEventConsumerManager<SendEmailService>(configuration, typeof(SendEmailService), nameof(SendEmailEvent));
        services.AddEmailManager(configuration);
    })
    .Build();

host.Run();