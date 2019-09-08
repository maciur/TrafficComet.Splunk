using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Accessors;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Enums;
using TrafficComet.Splunk.LogWriter.Extensions;

namespace TrafficComet.Splunk.LogWriter.Services
{
    public class TrafficCometSplunkHealthCheckerService : BackgroundService
    {
        protected const int MINUTES_MULTIPLIER = 60000;
        protected const int SECONDS_MULTIPLIER = 1000;
        protected const int WAIT_AND_CHECK_AGAIN = 10;

        protected IConfiguration Configuration { get; }
        protected ITrafficCometInternalHealthHttpClient HttpClient { get; }
        protected ILogger<TrafficCometSplunkHealthCheckerService> Logger { get; }

        public TrafficCometSplunkHealthCheckerService(ITrafficCometInternalHealthHttpClient splunkHttpCollectorHealthClient,
            ILogger<TrafficCometSplunkHealthCheckerService> logger, IConfiguration configuration)
        {
            HttpClient = splunkHttpCollectorHealthClient
                ?? throw new ArgumentNullException(nameof(splunkHttpCollectorHealthClient));

            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            Configuration = configuration
                ?? throw new ArgumentNullException(nameof(configuration));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool httpCollectorTurnOff = default;

                try
                {
                    httpCollectorTurnOff = Configuration.GetBool(ConfigurationSelectors.HTTP_COLLECTOR_TURN_OFF);

                    if (!httpCollectorTurnOff && !SplunkHealthStatusAccessor.IsHealthy)
                    {
                        await CheckHealthStatus();
                    }
                }
                catch (Exception ex)
                {
                    HttpClient.CancelPendingRequests();
                    Logger.LogCritical(ex, $"{nameof(TrafficCometSplunkHealthCheckerService)} throw Exception");
                }
                await Task.Delay(WAIT_AND_CHECK_AGAIN * (httpCollectorTurnOff ? MINUTES_MULTIPLIER : SECONDS_MULTIPLIER));
            }
        }

        private async Task CheckHealthStatus()
        {
            var splunkCollectorHealthStatus = await HttpClient.GetHealthStatusAsync();

            if (splunkCollectorHealthStatus?.Code == (int)HttpCollectorResponseStatus.IsHealthy)
            {
                SplunkHealthStatusAccessor.IsHealthAgain();
            }
        }
    }
}