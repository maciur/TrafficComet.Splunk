using Microsoft.Extensions.Logging;
using System;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Extensions
{
    internal static class LoggerMessagesExtensions
    {
        internal static void ContentNotSupported<T>(this ILogger<T> logger, string contentTypeName)
        {
            logger.LogWarning($"Splunk Delegating Handler: content not supported {contentTypeName}");
        }

        internal static void Error<T, TException>(this ILogger<T> logger, TException exception)
            where TException : Exception
        {
            logger.LogError(exception, exception.ToString());
        }
    }
}
