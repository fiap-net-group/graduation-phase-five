using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace TechBlog.Infrastructure.Logger.Boundaries
{
    public class ApplicationInsightsCloudRoleNameInitializer : ITelemetryInitializer
    {
        private readonly string _appName;

        public ApplicationInsightsCloudRoleNameInitializer(IConfiguration configuration)
        {
            _appName = configuration.GetValue("Logging:Configuration:ApplicationName", Assembly.GetEntryAssembly().FullName);

            if (string.IsNullOrWhiteSpace(_appName))
                throw new ArgumentException(_appName, nameof(configuration));
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _appName;
        }
    }
}
