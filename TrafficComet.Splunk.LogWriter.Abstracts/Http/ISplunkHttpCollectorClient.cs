using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Http
{
    public interface ISplunkHttpCollectorClient
	{
		Task<HttpCollectorResponseDocument> PostJsonAsync(string url, IndexEventContainerDocument indexEventContract);
	}
}