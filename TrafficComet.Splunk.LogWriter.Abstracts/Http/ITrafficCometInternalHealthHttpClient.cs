using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Http
{
    public interface ITrafficCometInternalHealthHttpClient
    {
        Task<HttpCollectorResponseDocument> GetHealthStatusAsync();
        void CancelPendingRequests();
    }
}