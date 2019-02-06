using TrafficComet.Abstracts.Logs;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Writers
{
	public interface IWebEventDocumentWriter
	{
		void WriteDocumentAsync(ITrafficLog trafficLog);
	}
}