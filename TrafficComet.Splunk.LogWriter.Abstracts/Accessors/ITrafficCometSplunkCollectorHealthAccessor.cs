namespace TrafficComet.Splunk.LogWriter.Abstracts.Accessors
{
    public interface ITrafficCometSplunkCollectorHealthAccessor
    {
        bool IsHealthy { get; }
        void MarkAsUnhealthy();
    }
}
