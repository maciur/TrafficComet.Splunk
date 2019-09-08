using System.Net.Http;
using System.Threading.Tasks;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Writers
{
    public interface IHttpResponseMessageLogWriter
    {
        ValueTask SaveAsync(HttpResponseMessage response, string source);
    }
}