using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Http
{
    public class TrafficCometInternalHealthHttpClient : ITrafficCometInternalHealthHttpClient
    {
        protected IConfiguration Configuration { get; }
        protected HttpClient HttpClient { get; }
        protected IHtttpCollectorResponseSplunkFactory HttpCollectorResponseFactory { get; }

        public TrafficCometInternalHealthHttpClient(HttpClient httpClient,
            IHtttpCollectorResponseSplunkFactory httpCollectorResponseFactory,
            IConfiguration configuration)
        {
            HttpClient = httpClient
                ?? throw new ArgumentNullException(nameof(httpClient));

            HttpCollectorResponseFactory = httpCollectorResponseFactory
                ?? throw new ArgumentNullException(nameof(httpCollectorResponseFactory));

            Configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<HttpCollectorResponseDocument> GetHealthStatusAsync()
        {
            var collectorHealthEndPoint = Configuration.GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_HEALTH_ENDPOINT);

            if (string.IsNullOrEmpty(collectorHealthEndPoint))
                throw new NullReferenceException(nameof(collectorHealthEndPoint));

            var httpResponseMessage = await HttpClient.GetAsync(collectorHealthEndPoint);

            if (httpResponseMessage == null)
                throw new NullReferenceException(nameof(httpResponseMessage));

            return await (httpResponseMessage.StatusCode == HttpStatusCode.OK
                ? HttpCollectorResponseFactory.CreateSuccessResponse(httpResponseMessage.Content)
                : HttpCollectorResponseFactory.CreateFailureResponse(httpResponseMessage.Content));
        }

        public void CancelPendingRequests()
        {
            if (HttpClient != null)
            {
                HttpClient.CancelPendingRequests();
            }
        }
    }
}