using TechBlog.Notification.API;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.AddDependencyInjection(builder.Configuration);

var app = builder.Build();

app.UseDependencyInjection(builder.Configuration);

app.Run();
