using TrafficComet.Abstracts.Logs;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Factories
{
	public interface IWebEventDocumentFactory
	{
		WebEventDocument Create(ITrafficLog trafficLog);
	}
}
