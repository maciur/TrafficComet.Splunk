using System.Collections.Generic;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Queues
{
    public interface IBackgroundWebEventsQueue
	{
        int Count { get; }

        IList<IndexEventContainerDocument> Dequeue(int howMany);

        IndexEventContainerDocument Dequeue();

        void Queue(IndexEventContainerDocument indexEvent);
    }
}