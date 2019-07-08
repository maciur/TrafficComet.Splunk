using System;
using TrafficComet.Splunk.LogWriter.Enums;

namespace TrafficComet.Splunk.LogWriter.Exceptions
{
    public class TrafficCometHttpCollectorException : Exception
    {
        public TrafficCometHttpCollectorException(HttpCollectorResponseStatus responseStatus)
            : base($"Splunk Http Collector response with code: {(int)responseStatus}")
        {

        }
    }
}
