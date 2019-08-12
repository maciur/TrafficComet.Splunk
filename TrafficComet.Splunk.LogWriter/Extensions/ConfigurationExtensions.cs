using Microsoft.Extensions.Configuration;
using System;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
    internal static class ConfigurationExtensions
    {
        internal static string GetString(this IConfiguration configuration, string configurationPath, string defaultValue = default(string))
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (string.IsNullOrEmpty(configurationPath))
                throw new ArgumentNullException(nameof(configurationPath));

            var value = configuration.GetValue<string>(configurationPath);

            return !string.IsNullOrEmpty(value) ? value : defaultValue;
        }

        internal static bool GetBool(this IConfiguration configuration, string configurationPath, bool defaultValue = default(bool))
        {
            var stringValue = configuration.GetString(configurationPath);
            return bool.TryParse(stringValue, out bool boolValue) ? boolValue : defaultValue;
        }
    }
}
