using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Config;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Http
{
    public class SplunkHttpCollectorHealthClient : ISplunkHttpCollectorHealthClient
    {
        protected IOptions<SplunkHttpCollectorConfig> Config { get; }
        protected HttpClient HttpClient { get; }
        protected IHtttpCollectorResponseSplunkFactory HttpCollectorResponseFactory { get; }

        public SplunkHttpCollectorHealthClient(HttpClient httpClient,
            IHtttpCollectorResponseSplunkFactory httpCollectorResponseFactory,
            IOptions<SplunkHttpCollectorConfig> splunkHttpCollectorConfig)
        {
            HttpClient = httpClient
                ?? throw new ArgumentNullException(nameof(httpClient));

            HttpCollectorResponseFactory = httpCollectorResponseFactory
                ?? throw new ArgumentNullException(nameof(httpCollectorResponseFactory));

            if (splunkHttpCollectorConfig == null || splunkHttpCollectorConfig.Value == null)
                throw new ArgumentNullException(nameof(splunkHttpCollectorConfig));

            Config = splunkHttpCollectorConfig;
        }

        public async Task<HttpCollectorResponseDocument> GetHealthStatusAsync()
        {
            var httpResponseMessage = await HttpClient.GetAsync(Config.Value.HealthEndPoint);

            if (httpResponseMessage == null)
                throw new NullReferenceException(nameof(httpResponseMessage));

            return await (httpResponseMessage.StatusCode == HttpStatusCode.OK
                ? HttpCollectorResponseFactory.CreateSuccessResponse(httpResponseMessage.Content)
                : HttpCollectorResponseFactory.CreateFailureResponse(httpResponseMessage.Content));
        }
    }
}