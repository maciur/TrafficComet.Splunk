using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using TrafficComet.Abstracts.Writers;
using TrafficComet.Core.Consts;
using TrafficComet.Core.Services;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Abstracts.Processor;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Config;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Extensions;
using TrafficComet.Splunk.LogWriter.Factories;
using TrafficComet.Splunk.LogWriter.Http;
using TrafficComet.Splunk.LogWriter.Processor;
using TrafficComet.Splunk.LogWriter.Queues;
using TrafficComet.Splunk.LogWriter.Writers;

namespace TrafficComet.Splunk.LogWriter.Installer
{
	public static class SplunkLogWriterInstaller
	{
		public static IServiceCollection AddTrafficCommetSplunkLogWriter(this IServiceCollection services,
			IConfiguration configuration)
		{
			var splunkConfigSection = configuration
				.GetSection($"{TrafficCometConstValues.RootConfigName}:{ConfigurationSelectors.SPLUNK_SELECTOR}");

			if (splunkConfigSection == null)
				throw new NullReferenceException(nameof(splunkConfigSection));

			var splunkCollectorsConfigSection = splunkConfigSection.GetSection(ConfigurationSelectors.COLLECTORS_SELECTOR);

			if (splunkCollectorsConfigSection == null)
				throw new NullReferenceException(nameof(splunkCollectorsConfigSection));

			var httpCollectorSection = splunkCollectorsConfigSection.GetSection(ConfigurationSelectors.HTTP_COLLECTOR_SELECTOR);

			if (httpCollectorSection == null)
				throw new NullReferenceException(nameof(httpCollectorSection));

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

			services.AddHostedService<ExecutorSaveLogTasksHostedService>();

			services.AddHostedService<SplunkHttpCollectorHealthCheckerHostedService>();

			services.AddSingleton<IBackgroundSaveLogTasksQueue, BackgroundSaveLogTasksQueue>();

			return services
				.Configure<ExecutorSaveLogTasksConfig>(splunkConfigSection
					.GetSection($"{ConfigurationSelectors.HOSTED_SERVICES_SELECTOR}:{ConfigurationSelectors.EXECUTOR_SELECTOR}"))
				.Configure<SplunkFolderCollectorConfig>(splunkCollectorsConfigSection
					.GetSection(ConfigurationSelectors.FOLDER_COLLECTOR_SELECTOR))
				.Configure<SplunkHttpCollectorConfig>(httpCollectorSection);
		}
	}
}