using System;
using System.Net.Http;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
    internal static class HttpClientExtensions
	{
        internal static HttpClient AddSplunkAuthHeader(this HttpClient httpClient, string authToken)
		{
			if (httpClient == null)
				throw new NullReferenceException(nameof(httpClient));

			if (string.IsNullOrEmpty(authToken))
				throw new ArgumentNullException(nameof(authToken));

			httpClient.DefaultRequestHeaders.Add("Authorization", $"Splunk {authToken}");
			return httpClient;
		}
	}
}