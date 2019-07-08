using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Queues
{
    public class BackgroundWebEventsQueue : IBackgroundWebEventsQueue
    {
        protected ConcurrentQueue<IndexEventContainerDocument> IndexEvents { get; }
        protected IApplicationLifetime ApplicationLifetime { get; }
        public int Count => IndexEvents.Count;

        private bool _canRegister = false;

        public BackgroundWebEventsQueue(IApplicationLifetime applicationLifetime)
        {
            IndexEvents = new ConcurrentQueue<IndexEventContainerDocument>();
            _canRegister = true;

            ApplicationLifetime = applicationLifetime
                ?? throw new ArgumentNullException(nameof(applicationLifetime));

            ApplicationLifetime.ApplicationStopping.Register(OnApplicationStopping);
        }

        public IList<IndexEventContainerDocument> Dequeue(int howMany)
        {
            if (IndexEvents.Count > 0)
            {
                return howMany > 1 ? DequeueMany(howMany) : DequeueSingle();
            }
            return default(List<IndexEventContainerDocument>);
        }

        public IndexEventContainerDocument Dequeue()
        {
            if (IndexEvents.Count > 0)
            {
                if (IndexEvents.TryDequeue(out var indexEvent))
                {
                    return indexEvent;
                }
            }
            return default(IndexEventContainerDocument);
        }

        public void Queue(IndexEventContainerDocument indexEvent)
        {
            if (_canRegister && indexEvent != null)
            {
                IndexEvents.Enqueue(indexEvent);
            }
        }

        private IList<IndexEventContainerDocument> DequeueMany(int howMany)
        {
            IList<IndexEventContainerDocument> indexEvents = new List<IndexEventContainerDocument>();

            for (int i = 0; i < howMany; i++)
            {
                if (IndexEvents.TryDequeue(out var indexEvent) && indexEvent != null)
                {
                    indexEvents.Add(indexEvent);
                }
            }
            return indexEvents;
        }

        private IList<IndexEventContainerDocument> DequeueSingle()
        {
            if (IndexEvents.TryDequeue(out var indexEvent) && indexEvent != null)
            {
                return new List<IndexEventContainerDocument> { indexEvent };
            }
            return default(List<IndexEventContainerDocument>);
        }

        private void OnApplicationStopping()
        {
            _canRegister = false;
        }
    }
}