using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TechBlog.Common.Api.Swagger
{
    [ExcludeFromCodeCoverage]
    internal static class SwaggerExtensions
    {
        internal static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                options.AddSecurity();

                options.CustomSchemaIds(type => type.FullName);

                options.IncludeCommentsToApiDocumentation();

                options.EnableAnnotations();
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }

        private static void AddSecurity(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });

            options.AddSecurityDefinition("Key", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter the valid Key",
                Name = "X-API-KEY",
                Type = SecuritySchemeType.ApiKey
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    Array.Empty<string>()
                },
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Key"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        }

        private static void IncludeCommentsToApiDocumentation(this SwaggerGenOptions options)
        {
            try
            {
                options.TryIncludeCommentsToApiDocumentation();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void TryIncludeCommentsToApiDocumentation(this SwaggerGenOptions options)
        {
            var xmlFile = $"{Assembly.GetEntryAssembly().GetName().Name}.xml";

            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.IncludeXmlComments(xmlPath);
        }


        internal static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            var provider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger(options =>
            {
                options.PreSerializeFilters.Add((document, request) =>
                {
                    document.Servers.Add(new()
                    {
                        Url = configuration.GetValue("Swagger:Server:Url", ""),
                        Description = configuration.GetValue("Swagger:Server:Description", "")
                    });

                    //document.ExternalDocs = new()
                    //{
                    //    Description = configuration.GetValue("Swagger:ExternalDocs:Description", ""),
                    //    Url = new Uri(configuration.GetValue("Swagger:ExternalDocs:Url", ""))    
                    //};
                });
            });

            app.UseSwaggerUI(
                options =>
                {

                    foreach (var groupName in provider.ApiVersionDescriptions.Select(description => description.GroupName))
                        options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
                });

            return app;
        }
    }
}
