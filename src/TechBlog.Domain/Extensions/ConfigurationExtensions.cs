using Microsoft.Extensions.Configuration;

namespace TechBlog.Domain.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetValueOrThrow<T>(this IConfiguration configuration, string key)
        {
            var response = configuration.GetValue<T>(key);

            if (response == null)
            {
                throw new ArgumentException("Configuration don't have key implemented", key);
            }

            return response;
        }

        public static CancellationToken GenerateCancellationToken(this IConfiguration configuration)
        {
            return new CancellationTokenSource().GenerateCancellationToken(configuration.GetValueOrThrow<int?>("Gateways:Event:Cancellation:Hours").Value,
                                                                           configuration.GetValueOrThrow<int?>("Gateways:Event:Cancellation:Minutes").Value,
                                                                           configuration.GetValueOrThrow<int?>("Gateways:Event:Cancellation:Seconds").Value);
        }

        public static bool IsProduction(this IConfiguration configuration)
        {
            return configuration.GetValue("Environment", "") == "Production";
        }
    }
}
