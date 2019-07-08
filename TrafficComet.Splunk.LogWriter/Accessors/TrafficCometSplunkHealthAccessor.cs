using TrafficComet.Splunk.LogWriter.Abstracts.Accessors;

namespace TrafficComet.Splunk.LogWriter.Accessors
{
    public class TrafficCometSplunkHealthAccessor : ITrafficCometSplunkCollectorHealthAccessor
    {
        public bool IsHealthy => SplunkHealthStatusAccessor.IsHealthy;

        public void MarkAsUnhealthy()
        {
            SplunkHealthStatusAccessor.StartCheckingHealthStatus();
        }
    }
}
