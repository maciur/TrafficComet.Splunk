using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Factories
{
    public interface IWebEventBodyDocumentFactory
	{
		WebEventBodyDocument Create(string fullUrl, dynamic requestBody,
			string clientId, string traceId);
	}
}