using System;
using System.Net.Http;
using TrafficComet.Splunk.LogWriter.Documents;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Contents;

namespace TrafficComet.Splunk.LogWriter.Factories
{
    public class HttpRequestMessageSplunkFactory : IHttpRequestMessageSplunkFactory
    {
        public HttpRequestMessage Create(HttpMethod httpMethod, string url,
            IndexEventContainerDocument indexEventContract)
        {
            if (httpMethod == null)
                throw new ArgumentNullException(nameof(httpMethod));

            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (indexEventContract == null)
                throw new ArgumentNullException(nameof(indexEventContract));

            var httpRequestMessage = new HttpRequestMessage(httpMethod, url)
            {
                Content = new JsonContent(indexEventContract)
            };
            return httpRequestMessage;
        }
    }
}