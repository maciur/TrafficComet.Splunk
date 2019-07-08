using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Accessors;
using TrafficComet.Splunk.LogWriter.Enums;

namespace TrafficComet.Splunk.LogWriter.Services
{
    public class TrafficCometSplunkHealthCheckerService : BackgroundService
    {
        protected const int WAIT_SECONDS_AND_CHECK_AGAIN = 5;
        protected ITrafficCometInternalHealthHttpClient HttpClient { get; }
        protected ILogger<TrafficCometSplunkHealthCheckerService> Logger { get; }

        public TrafficCometSplunkHealthCheckerService(ITrafficCometInternalHealthHttpClient splunkHttpCollectorHealthClient,
            ILogger<TrafficCometSplunkHealthCheckerService> logger)
        {
            HttpClient = splunkHttpCollectorHealthClient
                ?? throw new ArgumentNullException(nameof(splunkHttpCollectorHealthClient));

            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (!SplunkHealthStatusAccessor.IsHealthy)
                    {
                        await CheckHealthStatus();
                    }
                }
                catch (Exception ex)
                {
                    HttpClient.CancelPendingRequests();
                    Logger.LogCritical(ex, $"{nameof(TrafficCometSplunkHealthCheckerService)} throw Exception");
                }
                await Task.Delay(WAIT_SECONDS_AND_CHECK_AGAIN * 1000);
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