using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;

namespace TrafficComet.Core.Services
{
    public class ExecutorSaveLogTasksHostedService : BackgroundService
    {
        public IBackgroundSaveLogTasksQueue TaskQueue { get; }
        protected ILogger<ExecutorSaveLogTasksHostedService> Logger { get; }

        protected const int TASKS_AT_ONES = 3;

        public ExecutorSaveLogTasksHostedService(IBackgroundSaveLogTasksQueue taskQueue,
            ILogger<ExecutorSaveLogTasksHostedService> logger)
        {
            TaskQueue = taskQueue;

            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug("Executor Save Log Tasks Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItems = await TaskQueue.DequeueAsync(cancellationToken, TASKS_AT_ONES);

                if(workItems != null && workItems.Any())
                {
                    try
                    {
                        await Task.WhenAll(workItems.Select(x => x(cancellationToken)));
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, $"Error occurred executing {nameof(workItems)}.");
                    }
                }
            }

            Logger.LogDebug("Executor Save Log Tasks Hosted Service is stopping.");
        }
    }
}