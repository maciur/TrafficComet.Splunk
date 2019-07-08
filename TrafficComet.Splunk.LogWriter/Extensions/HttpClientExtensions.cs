using System;
using System.Net.Http;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
    internal static class HttpClientExtensions
    {
        private const string AUTH_SPLUNK_HEADER = "Authorization";
        private const string SPLUNK_TOKEN_PATTERN = "Splunk {0}";

        internal static HttpClient AddSplunkAuthHeader(this HttpClient httpClient, string authToken)
        {
            if (httpClient == null)
                throw new NullReferenceException(nameof(httpClient));

            if (string.IsNullOrEmpty(authToken))
                throw new ArgumentNullException(nameof(authToken));

            if (!httpClient.DefaultRequestHeaders.Contains(AUTH_SPLUNK_HEADER))
            {
                httpClient.DefaultRequestHeaders.Add(AUTH_SPLUNK_HEADER,
                    string.Format(SPLUNK_TOKEN_PATTERN, authToken));
            }

            return httpClient;
        }
    }
}