using System;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Exceptions
{
    public class TrafficCometFailedSaveWebEventException : Exception
    {
        public IndexEventContainerDocument WebEvent { get; }

        public TrafficCometFailedSaveWebEventException(IndexEventContainerDocument webEvent) 
            : base($"Failed to save Web Event in all places")
        {
            WebEvent = webEvent
                ?? throw new ArgumentNullException(nameof(webEvent));
        }
    }
}
