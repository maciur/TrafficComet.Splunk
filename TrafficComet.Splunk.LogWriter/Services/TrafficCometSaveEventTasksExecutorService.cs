using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Enums;
using TrafficComet.Splunk.LogWriter.Extensions;
using TrafficComet.Splunk.LogWriter.Tasks;

namespace TrafficComet.Splunk.LogWriter.Services
{
    public class TrafficCometSaveEventTasksExecutorService : BackgroundService
    {
        protected const int DEFAULT_EVENTS_AT_ONES = 5;
        protected const int WAIT_SEC_BEFORE_EXECUTE_NEXT_TASKS = 3;
        protected const int WAIT_SEC_BEFORE_TRY_AGAIN = 5;
        protected IApplicationLifetime ApplicationLifetime { get; }
        protected IConfiguration Configuration { get; }

        protected int EventsAtOnes
        {
            get
            {
                var eventsAtOnes = Configuration.GetValue<int>(ConfigurationSelectors.SERVICES_EVENTS_AT_ONES);
                return eventsAtOnes <= 0 ? DEFAULT_EVENTS_AT_ONES : eventsAtOnes;
            }
        }

        protected ILogger<TrafficCometSaveEventTasksExecutorService> Logger { get; }
        protected IServiceProvider ServiceProvider { get; }
        protected IBackgroundWebEventsQueue WebEventsQueue { get; }

        public TrafficCometSaveEventTasksExecutorService(IBackgroundWebEventsQueue webEventsQueue,
            IConfiguration configuration, IServiceProvider serviceProvider, IApplicationLifetime applicationLifetime,
            ILogger<TrafficCometSaveEventTasksExecutorService> logger)
        {
            WebEventsQueue = webEventsQueue
                ?? throw new ArgumentNullException(nameof(webEventsQueue));

            Configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));

            ServiceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));

            ApplicationLifetime = applicationLifetime
                ?? throw new ArgumentNullException(nameof(applicationLifetime));

            ApplicationLifetime.ApplicationStopping.Register(OnApplicationStopping);

            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void ConvertSaveTasks<TConvertFrom, TConvertTo>(ref IList<WebEventSaveTask> saveTasks)
            where TConvertFrom : WebEventSaveTask
            where TConvertTo : WebEventSaveTask
        {
            if (saveTasks.SafeAny())
            {
                for (int i = 0; i < saveTasks.Count; i++)
                {
                    if (saveTasks[i].ReadyToConvert && saveTasks[i] is TConvertFrom)
                    {
                        var fileSaveTask = ServiceProvider.GetService<TConvertTo>();
                        fileSaveTask.LoadIndexEvent(saveTasks[i].WebEvent);
                        saveTasks[i] = fileSaveTask;
                    }
                }
            }
        }

        protected void CreateNewSaveTasks(int howManyEventsTasks, ref IList<WebEventSaveTask> saveTasks)
        {
            if (howManyEventsTasks > 0)
            {
                var indexEvents = WebEventsQueue.Dequeue(howManyEventsTasks);
                if (indexEvents.SafeAny())
                {
                    foreach (var indexEvent in indexEvents)
                    {
                        var saveTask = ServiceProvider.GetService<WebEventHttpSaveTask>();
                        saveTask.LoadIndexEvent(indexEvent);

                        if (saveTask.Status != WebEventSaveTaskStatus.Ignore)
                        {
                            saveTasks.Add(saveTask);
                        }
                    }
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                IList<WebEventSaveTask> saveTasks = new List<WebEventSaveTask>();

                try
                {
                    CreateNewSaveTasks(EventsAtOnes, ref saveTasks);

                    while (saveTasks.SafeAny())
                    {
                        _ = await Task.WhenAll(saveTasks.Select(async x =>
                        {
                            return await x.ExecuteAsync();
                        }));

                        saveTasks = saveTasks
                            .Where(x => x.Status != WebEventSaveTaskStatus.Ok && x.Status != WebEventSaveTaskStatus.Ignore)
                            .ToList();

                        ConvertSaveTasks<WebEventHttpSaveTask, WebEventFileSaveTask>(ref saveTasks);
                        ConvertSaveTasks<WebEventFileSaveTask, WebEventExceptionSaveTask>(ref saveTasks);
                        await Task.Delay(WAIT_SEC_BEFORE_TRY_AGAIN * 1000);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                }

                await Task.Delay(WAIT_SEC_BEFORE_TRY_AGAIN * 1000);
            }
        }

        private void OnApplicationStopping()
        {
            while (WebEventsQueue.Count > 0)
            {
                var indexEvents = WebEventsQueue.Dequeue(20);
                Parallel.ForEach(indexEvents, indexEvent =>
                {
                    try
                    {
                        var saveTask = ServiceProvider.GetService<WebEventFileSaveTask>();
                        if (saveTask != null)
                        {
                            saveTask.LoadIndexEvent(indexEvent);
                            saveTask.SaveLog();
                        }
                    }
                    catch
                    {
                        // do nothing with it.
                    }
                });
            }
        }
    }
}