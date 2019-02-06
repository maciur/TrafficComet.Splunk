using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Factories
{
	public interface IHtttpCollectorResponseSplunkFactory
	{
		Task<HttpCollectorResponseDocument> CreateFailureResponse(HttpContent httpContent);

		Task<HttpCollectorResponseDocument> CreateSuccessResponse(HttpContent httpContent);
	}
}
