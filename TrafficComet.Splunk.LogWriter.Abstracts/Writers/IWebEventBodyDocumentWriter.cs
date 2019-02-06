using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Writers
{
    public interface IWebEventBodyDocumentWriter
    {
        void Write(string url, dynamic requestObject, string sourceName, string clientId,
               string traceId, IndexEventSplunkType indexEventSplunkType);

        void Write(WebEventBodyDocument webEventBodyDocument, string sourceName, 
            IndexEventSplunkType indexEventSplunkType);
    }
}