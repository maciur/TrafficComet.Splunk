using System.Net.Http;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Factories
{
    public interface IHttpRequestMessageSplunkFactory
	{
		HttpRequestMessage Create(HttpMethod httpMethod, string url,
            IndexEventContainerDocument indexEventContract);
	}
}