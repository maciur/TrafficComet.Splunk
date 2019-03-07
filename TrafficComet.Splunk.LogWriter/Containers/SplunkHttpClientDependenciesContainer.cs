using Microsoft.Extensions.Options;
using System;
using TrafficComet.Abstracts;
using TrafficComet.Abstracts.Accessors;
using TrafficComet.Core.Configs;
using TrafficComet.Splunk.LogWriter.Abstracts.Containers;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;

namespace TrafficComet.Splunk.LogWriter.Containers
{
	public class SplunkHttpClientDependenciesContainer : ISplunkHttpClientDependenciesContainer
	{
		public IClientIdGenerator ClientIdGenerator { get; }

		public ITraceIdGenerator TraceIdGenerator { get; }

		public IWebEventBodyDocumentFactory WebEventBodyDocumentFactory { get; }

		public IWebEventBodyDocumentWriter WebEventBodyDocumentWriter { get; }

		public ITrafficCometMiddlewaresAccessor TrafficCometMiddlewaresAccessor { get; }

        public IOptionsSnapshot<TrafficCometMiddlewareConfig> MiddlewareConfig { get; }

        public bool StopLogging => MiddlewareConfig?.Value?.StopLogging ?? false;

        public SplunkHttpClientDependenciesContainer(IClientIdGenerator clientIdGenerator,
			ITraceIdGenerator traceIdGenerator, IWebEventBodyDocumentFactory webEventBodyDocumentFactory,
			IWebEventBodyDocumentWriter webEventBodyDocumentWriter, ITrafficCometMiddlewaresAccessor trafficCometMiddlewaresAccessor,
            IOptionsSnapshot<TrafficCometMiddlewareConfig> trafficCometMiddlewareConfig)
		{
			ClientIdGenerator = clientIdGenerator
				?? throw new ArgumentNullException(nameof(clientIdGenerator));

			TraceIdGenerator = traceIdGenerator
				?? throw new ArgumentNullException(nameof(traceIdGenerator));

			WebEventBodyDocumentFactory = webEventBodyDocumentFactory
				?? throw new ArgumentNullException(nameof(webEventBodyDocumentFactory));

			WebEventBodyDocumentWriter = webEventBodyDocumentWriter
				?? throw new ArgumentNullException(nameof(webEventBodyDocumentWriter));

			TrafficCometMiddlewaresAccessor = trafficCometMiddlewaresAccessor
				?? throw new ArgumentNullException(nameof(trafficCometMiddlewaresAccessor));

            MiddlewareConfig = trafficCometMiddlewareConfig
                ?? throw new ArgumentNullException(nameof(MiddlewareConfig));

        }
	}
}