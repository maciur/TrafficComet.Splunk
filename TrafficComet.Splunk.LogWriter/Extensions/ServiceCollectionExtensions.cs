using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TrafficComet.Splunk.LogWriter.Consts;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static void AddHttpClient<TClient, TImplementation>(this IServiceCollection services,
            IConfiguration configuration, bool addToken = false, TimeSpan? timeout = null)
            where TClient : class
            where TImplementation : class, TClient
        {
            services.AddHttpClient<TClient, TImplementation>((client) =>
            {
                var splunkUrl = configuration.GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_URL);

                if (string.IsNullOrEmpty(splunkUrl))
                    throw new NullReferenceException(nameof(splunkUrl));

                client.BaseAddress = new Uri(splunkUrl);

                if (timeout != null && timeout.HasValue)
                {
                    client.Timeout = timeout.Value;
                }

                if (addToken)
                {
                    var serviceToken = configuration.GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_TOKEN);

                    if (string.IsNullOrEmpty(serviceToken))
                        throw new NullReferenceException(nameof(serviceToken));

                    client.AddSplunkAuthHeader(serviceToken);
                }
            });
        }
    }
}
