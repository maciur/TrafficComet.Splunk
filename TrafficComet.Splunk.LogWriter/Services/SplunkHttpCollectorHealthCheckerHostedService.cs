using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Accessors;

namespace TrafficComet.Core.Services
{
    public class SplunkHttpCollectorHealthCheckerHostedService : BackgroundService
    {
        protected ISplunkHttpCollectorHealthClient HttpClient { get; }
        protected ILogger<SplunkHttpCollectorHealthCheckerHostedService> Logger { get; }

        protected const int IS_HEALTHY_CODE = 17;
        protected const int WAIT_SECONDS_AND_CHECK_AGAIN = 5;

        public SplunkHttpCollectorHealthCheckerHostedService(ISplunkHttpCollectorHealthClient splunkHttpCollectorHealthClient,
            ILogger<SplunkHttpCollectorHealthCheckerHostedService> logger)
        {
            HttpClient = splunkHttpCollectorHealthClient
                ?? throw new ArgumentNullException(nameof(splunkHttpCollectorHealthClient));

            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Logger.LogDebug("Executor Save Log Tasks Hosted Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (!SplunkHttpCollectorHealthStatusAccessor.IsHealthy)
                {
                    await CheckHealthStatus();
                }
                Thread.Sleep(TimeSpan.FromSeconds(WAIT_SECONDS_AND_CHECK_AGAIN));
            }

            Logger.LogDebug("Executor Save Log Tasks Hosted Service is stopping.");
        }

        protected async Task CheckHealthStatus()
        {
            try
            {
                var splunkCollectorHealthStatus = await HttpClient.GetHealthStatusAsync();

                if (splunkCollectorHealthStatus?.Code == IS_HEALTHY_CODE)
                {
                    SplunkHttpCollectorHealthStatusAccessor.IsHealthAgain();
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromSeconds(WAIT_SECONDS_AND_CHECK_AGAIN));
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error occurred executing check health status.");
            }
        }
    }
}