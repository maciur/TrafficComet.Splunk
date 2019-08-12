using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TrafficComet.Abstracts.Writers;
using TrafficComet.Core;
using TrafficComet.Splunk.LogWriter.Abstracts.Accessors;
using TrafficComet.Splunk.LogWriter.Abstracts.Containers;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Accessors;
using TrafficComet.Splunk.LogWriter.Containers;
using TrafficComet.Splunk.LogWriter.Extensions;
using TrafficComet.Splunk.LogWriter.Factories;
using TrafficComet.Splunk.LogWriter.Http;
using TrafficComet.Splunk.LogWriter.Queues;
using TrafficComet.Splunk.LogWriter.Services;
using TrafficComet.Splunk.LogWriter.Tasks;
using TrafficComet.Splunk.LogWriter.Writers;

namespace TrafficComet.Splunk.LogWriter.Installer
{
    public static class SplunkLogWriterInstaller
    {
        public static IServiceCollection AddTrafficCometSplunkHealthChecker(this IServiceCollection service)
        {
            return service
                .AddTransient<ITrafficCometSplunkCollectorHealthAccessor, TrafficCometSplunkHealthAccessor>()
                .AddHostedService<TrafficCometSplunkHealthCheckerService>();
        }

        public static IServiceCollection AddTrafficCometSplunk(this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.AddTrafficCometSplunk(configuration, false);
        }

        public static IServiceCollection AddTrafficCometSplunk(this IServiceCollection services,
            IConfiguration configuration, bool readTraceIdAndClientIfFromHeader)
        {
            return services
                .AddTrafficComet(configuration, readTraceIdAndClientIfFromHeader)
                .AddHttpClients(configuration)
                .AddWriters()
                .AddFactories()
                .AddContainers()
                .AddSaveTasks()
                .AddBackgroundElements();
        }

        private static IServiceCollection AddBackgroundElements(this IServiceCollection service)
        {
            return service
                .AddSingleton<IBackgroundWebEventsQueue, BackgroundWebEventsQueue>()
                .AddHostedService<TrafficCometSaveEventTasksExecutorService>();
        }

        private static IServiceCollection AddContainers(this IServiceCollection service)
        {
            service.TryAddTransient<ISplunkHttpClientDependenciesContainer, SplunkHttpClientDependenciesContainer>();
            return service;
        }

        private static IServiceCollection AddFactories(this IServiceCollection service)
        {
            service.TryAddSingleton<IHttpRequestMessageSplunkFactory, HttpRequestMessageSplunkFactory>();
            service.TryAddSingleton<IHtttpCollectorResponseSplunkFactory, HtttpCollectorResponseSplunkFactory>();
            service.TryAddSingleton<IWebEventDocumentFactory, WebEventDocumentFactory>();
            service.TryAddSingleton<IWebEventBodyDocumentFactory, WebEventBodyDocumentFactory>();
            service.TryAddSingleton<IIndexEventContainerDocumentFactory, IndexEventContainerDocumentFactory>();

            return service
                .AddTransient<IIndexEventContainerDocumentFactory, IndexEventContainerDocumentFactory>()
                .AddTransient<IHttpRequestMessageSplunkFactory, HttpRequestMessageSplunkFactory>();
        }

        private static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ITrafficCometInternalHttpClient, TrafficCometInternalHttpClient>(configuration, true,
                TimeSpan.FromMinutes(1));

            services.AddHttpClient<ITrafficCometInternalHealthHttpClient, TrafficCometInternalHealthHttpClient>
                (configuration, timeout: TimeSpan.FromMinutes(1));

            return services;
        }

        private static IServiceCollection AddSaveTasks(this IServiceCollection service)
        {
            return service
                .AddTransient<WebEventHttpSaveTask>()
                .AddTransient<WebEventExceptionSaveTask>()
                .AddTransient<WebEventFileSaveTask>();
        }

        private static IServiceCollection AddWriters(this IServiceCollection services)
        {
            services.TryAddTransient<ITrafficLogWriter, TrafficCometSplunkLogWriter>();
            services.TryAddTransient<IWebEventDocumentWriter, WebEventDocumentWriter>();
            services.TryAddTransient<IWebEventBodyDocumentWriter, WebEventBodyDocumentWriter>();
            return services;
        }
    }
}