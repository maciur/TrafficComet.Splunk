using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TrafficComet.Abstracts.Writers;
using TrafficComet.Core;
using TrafficComet.Core.Services;
using TrafficComet.Splunk.LogWriter.Abstracts.Containers;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Abstracts.Processor;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Config;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Containers;
using TrafficComet.Splunk.LogWriter.Extensions;
using TrafficComet.Splunk.LogWriter.Factories;
using TrafficComet.Splunk.LogWriter.Http;
using TrafficComet.Splunk.LogWriter.Processor;
using TrafficComet.Splunk.LogWriter.Queues;
using TrafficComet.Splunk.LogWriter.Writers;
using TrafficConfigurationSelectors = TrafficComet.Abstracts.Consts.ConfigurationSelectors;

namespace TrafficComet.Splunk.LogWriter.Installer
{
	public static class SplunkLogWriterInstaller
	{
		public static IServiceCollection AddTrafficCometSplunkLogWriter(this IServiceCollection services, 
			IConfiguration configuration)
		{
			return services.AddTrafficCometSplunkLogWriter(configuration, false);
		}

		public static IServiceCollection AddTrafficCometSplunkLogWriter(this IServiceCollection services,
			IConfiguration configuration, bool readTraceIdAndClientIfFromHeader)
		{
			var splunkConfigSectionPath = string.Join(':', TrafficConfigurationSelectors.ROOT,
				TrafficConfigurationSelectors.WRITERS, ConfigurationSelectors.SPLUNK);

			var splunkConfigSection = configuration.GetSection(splunkConfigSectionPath);

			if (splunkConfigSection == null)
				throw new NullReferenceException(nameof(splunkConfigSection));

			var splunkCollectorsConfigSection = splunkConfigSection.GetSection(ConfigurationSelectors.COLLECTORS);

			if (splunkCollectorsConfigSection == null)
				throw new NullReferenceException(nameof(splunkCollectorsConfigSection));

			var httpCollectorSection = splunkCollectorsConfigSection.GetSection(ConfigurationSelectors.HTTP_COLLECTOR);

			if (httpCollectorSection == null)
				throw new NullReferenceException(nameof(httpCollectorSection));

			services.AddTrafficComet(configuration, readTraceIdAndClientIfFromHeader);

			services.AddHttpClient<ISplunkHttpCollectorClient, SplunkHttpCollectorClient>(client =>
			{
				var httpCollectorConfig = httpCollectorSection.Get<SplunkHttpCollectorConfig>();

				if (httpCollectorConfig == null)
					throw new NullReferenceException(nameof(httpCollectorConfig));

				client.BaseAddress = new Uri(httpCollectorConfig.Url);
				client.AddSplunkAuthHeader(httpCollectorConfig.Token);
			});

			services.AddHttpClient<ISplunkHttpCollectorHealthClient, SplunkHttpCollectorHealthClient>(client =>
			{
				var httpCollectorConfig = httpCollectorSection.Get<SplunkHttpCollectorConfig>();

				if (httpCollectorConfig == null)
					throw new NullReferenceException(nameof(httpCollectorConfig));

				client.BaseAddress = new Uri(httpCollectorConfig.Url);
			});

			services.TryAddSingleton
				<IHttpRequestMessageSplunkFactory, HttpRequestMessageSplunkFactory>();

			services.TryAddSingleton
				<IHtttpCollectorResponseSplunkFactory, HtttpCollectorResponseSplunkFactory>();

			services.TryAddSingleton
				<IWebEventDocumentFactory, WebEventDocumentFactory>();

			services.TryAddSingleton
				<IWebEventBodyDocumentFactory, WebEventBodyDocumentFactory>();

			services.TryAddSingleton
				<IIndexEventContainerDocumentFactory, IndexEventContainerDocumentFactory>();

			services.TryAddTransient<ITrafficLogWriter, SplunkLogWriter>();

			services.TryAddTransient<IWebEventDocumentWriter, WebEventDocumentWriter>();

			services.TryAddTransient<IWebEventBodyDocumentWriter, WebEventBodyDocumentWriter>();

			services.TryAddTransient<ISplunkIndexEventProcessor, SplunkIndexEventProcessor>();

			services.TryAddTransient<ISplunkHttpClientDependenciesContainer, SplunkHttpClientDependenciesContainer>();

			services.AddHostedService<ExecutorSaveLogTasksHostedService>();

			services.AddHostedService<SplunkHttpCollectorHealthCheckerHostedService>();

			services.AddSingleton<IBackgroundSaveLogTasksQueue, BackgroundSaveLogTasksQueue>();

			return services
				.Configure<ExecutorSaveLogTasksConfig>(splunkConfigSection
					.GetSection($"{ConfigurationSelectors.HOSTED_SERVICES}:{ConfigurationSelectors.EXECUTOR}"))
				.Configure<SplunkFolderCollectorConfig>(splunkCollectorsConfigSection
					.GetSection(ConfigurationSelectors.FOLDER_COLLECTOR))
				.Configure<SplunkHttpCollectorConfig>(httpCollectorSection);
		}
	}
}