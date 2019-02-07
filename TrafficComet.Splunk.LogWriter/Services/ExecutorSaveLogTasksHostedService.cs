using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Config;

namespace TrafficComet.Core.Services
{
    public class ExecutorSaveLogTasksHostedService : BackgroundService
    {
        protected IBackgroundSaveLogTasksQueue TaskQueue { get; }
		protected IOptions<ExecutorSaveLogTasksConfig> Config { get; }  
		protected ILogger<ExecutorSaveLogTasksHostedService> Logger { get; }

		protected const int DEFAULT_TASKS_AT_ONES = 5;

		protected int TasksAtOnes => Config.Value?.TasksAtOnce <= 0 ?
			DEFAULT_TASKS_AT_ONES : Config.Value.TasksAtOnce;

		public ExecutorSaveLogTasksHostedService(IBackgroundSaveLogTasksQueue taskQueue,
            ILogger<ExecutorSaveLogTasksHostedService> logger,
			IOptions<ExecutorSaveLogTasksConfig> executorSaveLogTasksConfig)
        {
            TaskQueue = taskQueue
				?? throw new ArgumentNullException(nameof(taskQueue));

			Logger = logger
				?? throw new ArgumentNullException(nameof(logger));

			if (executorSaveLogTasksConfig == null || executorSaveLogTasksConfig.Value == null)
				throw new ArgumentNullException(nameof(executorSaveLogTasksConfig));

			Config = executorSaveLogTasksConfig;
		}

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug("Executor Save Log Tasks Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
				var workItems = await TaskQueue.DequeueAsync(cancellationToken, TasksAtOnes);

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