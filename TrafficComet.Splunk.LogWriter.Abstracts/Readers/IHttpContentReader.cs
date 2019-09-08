using System.Net.Http;
using System.Threading.Tasks;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Readers
{
    public interface IHttpContentReader
    {
        Task<(bool result, dynamic contentValue)> ReadAsync(HttpContent httpContent);
    }
}
