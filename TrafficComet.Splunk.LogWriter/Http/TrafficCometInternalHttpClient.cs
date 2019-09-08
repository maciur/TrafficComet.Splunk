using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Http
{
    public class TrafficCometInternalHttpClient : ITrafficCometInternalHttpClient
    {
        protected HttpClient HttpClient { get; }
        protected IHtttpCollectorResponseSplunkFactory HttpCollectorResponseFactory { get; }
        protected IHttpRequestMessageSplunkFactory HttpRequestMessageFactory { get; }

        public TrafficCometInternalHttpClient(HttpClient httpClient,
            IHttpRequestMessageSplunkFactory httpRequestMessageSplunkFactory,
            IHtttpCollectorResponseSplunkFactory htttpCollectorResponseFactory)
        {
            HttpClient = httpClient
                ?? throw new ArgumentNullException(nameof(httpClient));

            HttpRequestMessageFactory = httpRequestMessageSplunkFactory
                ?? throw new ArgumentNullException(nameof(httpRequestMessageSplunkFactory));

            HttpCollectorResponseFactory = htttpCollectorResponseFactory
                ?? throw new ArgumentNullException(nameof(htttpCollectorResponseFactory));
        }

        public async Task<HttpCollectorResponseDocument> PostJsonAsync(string url, IndexEventContainerDocument indexEventContract)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (indexEventContract == null)
                throw new ArgumentNullException(nameof(indexEventContract));

            using (var httpResponseMessage = await SendJsonAsync(HttpMethod.Post, url, indexEventContract))
            {
                if (httpResponseMessage == null)
                    throw new NullReferenceException(nameof(httpResponseMessage));

                return await ReadJsonResponseAsync(httpResponseMessage);
            }
        }

        protected virtual Task<HttpCollectorResponseDocument> ReadJsonResponseAsync(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage == null)
                throw new NullReferenceException(nameof(httpResponseMessage));

            return httpResponseMessage.StatusCode == HttpStatusCode.OK
                ? HttpCollectorResponseFactory.CreateSuccessResponse(httpResponseMessage.Content)
                : HttpCollectorResponseFactory.CreateFailureResponse(httpResponseMessage.Content);
        }

        protected virtual Task<HttpResponseMessage> SendJsonAsync(HttpMethod httpMethod,
            string url, IndexEventContainerDocument indexEventContract)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (indexEventContract == null)
                throw new ArgumentNullException(nameof(indexEventContract));

            var httpRequestMessage = HttpRequestMessageFactory.Create(httpMethod, url, indexEventContract);

            if (httpRequestMessage == null)
                throw new ArgumentNullException(nameof(httpRequestMessage));

            return HttpClient.SendAsync(httpRequestMessage);
        }
    }
}