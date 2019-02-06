using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Processor
{
    public interface ISplunkIndexEventProcessor
    {
        void ProcessIndexEvent(IndexEventContainerDocument indexEventSplunkContract);
    }
}