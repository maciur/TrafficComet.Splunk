using TrafficComet.Abstracts;
using TrafficComet.Abstracts.Accessors;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Containers
{
	public interface ISplunkHttpClientDependenciesContainer
	{
		IClientIdGenerator ClientIdGenerator { get; }
		ITraceIdGenerator TraceIdGenerator { get; }
		IWebEventBodyDocumentFactory WebEventBodyDocumentFactory { get; }
		IWebEventBodyDocumentWriter WebEventBodyDocumentWriter { get; }
		ITrafficCometMiddlewaresAccessor TrafficCometMiddlewaresAccessor { get; }
	}
}
