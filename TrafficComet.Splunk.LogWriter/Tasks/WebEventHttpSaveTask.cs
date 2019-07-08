using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Accessors;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Enums;
using TrafficComet.Splunk.LogWriter.Exceptions;

namespace TrafficComet.Splunk.LogWriter.Tasks
{
    public class WebEventHttpSaveTask : WebEventSaveTask
    {
        protected ITrafficCometInternalHttpClient SplunkHttpCollectorClient { get; }
        protected IConfiguration Configuration { get; }
        protected ITrafficCometSplunkCollectorHealthAccessor SplunkCollectorHealthAccessor { get; }

        public WebEventHttpSaveTask(ILogger<WebEventHttpSaveTask> logger,
            ITrafficCometInternalHttpClient splunkHttpCollectorClient, IConfiguration configuration,
            ITrafficCometSplunkCollectorHealthAccessor splunkCollectorHealthAccessor)
            : base(logger)
        {
            SplunkHttpCollectorClient = splunkHttpCollectorClient
                ?? throw new ArgumentNullException(nameof(splunkHttpCollectorClient));

            Configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));

            SplunkCollectorHealthAccessor = splunkCollectorHealthAccessor
                ?? throw new ArgumentNullException(nameof(splunkCollectorHealthAccessor));
        }

        protected override ValueTask<bool> SaveTaskAsync()
        {
            if (WebEvent == null)
                throw new ArgumentNullException(nameof(WebEvent));

            return SplunkCollectorHealthAccessor.IsHealthy ?
                new ValueTask<bool>(SendToCollectorAsync())
                : new ValueTask<bool>(false);
        }

        protected async Task<bool> SendToCollectorAsync()
        {
            var splunkCollectorEndPoint = Configuration.GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_ENDPOINT);

            if (string.IsNullOrEmpty(splunkCollectorEndPoint))
                throw new NullReferenceException(nameof(splunkCollectorEndPoint));

            var httpCollectorResponse = await SplunkHttpCollectorClient
                .PostJsonAsync(splunkCollectorEndPoint, WebEvent);

            if (httpCollectorResponse == null)
                throw new NullReferenceException(nameof(httpCollectorResponse));

            return HandleCollectorStatus((HttpCollectorResponseStatus)httpCollectorResponse.Code);
        }

        protected bool HandleCollectorStatus(HttpCollectorResponseStatus collectorStatus)
        {
            if (collectorStatus != HttpCollectorResponseStatus.Success)
            {
                if (collectorStatus == HttpCollectorResponseStatus.InternalServerError
                    || collectorStatus == HttpCollectorResponseStatus.ServerIsBusy)
                {
                    SplunkCollectorHealthAccessor.MarkAsUnhealthy();
                }
                else
                {
                    DoNotTryAgain();
                    throw new TrafficCometHttpCollectorException(collectorStatus);
                }
                return false;
            }
            return true;
        }
    }
}
