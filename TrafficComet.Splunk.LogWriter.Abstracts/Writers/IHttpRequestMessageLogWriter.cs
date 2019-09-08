using System.Net.Http;
using System.Threading.Tasks;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Writers
{
    public interface IHttpRequestMessageLogWriter
    {
        ValueTask SaveAsync(HttpRequestMessage request, string source);
    }
}