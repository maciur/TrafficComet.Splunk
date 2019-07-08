using System;
using TrafficComet.Abstracts.Logs;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;

namespace TrafficComet.Splunk.LogWriter.Writers
{
    public class WebEventDocumentWriter : IWebEventDocumentWriter
    {
        protected IIndexEventContainerDocumentFactory IndexEventSplunkContractFactory { get; }
        protected IWebEventDocumentFactory WebEventDocumentFactory { get; }
        protected IBackgroundWebEventsQueue BackgroundWebEventsQueue { get; }

        public WebEventDocumentWriter(IIndexEventContainerDocumentFactory indexEventSplunkContractFactory,
            IWebEventDocumentFactory webEventDocumentFactory, IBackgroundWebEventsQueue backgroundWebEventsQueue)
        {
            IndexEventSplunkContractFactory = indexEventSplunkContractFactory
                ?? throw new ArgumentNullException(nameof(indexEventSplunkContractFactory));

            WebEventDocumentFactory = webEventDocumentFactory
                ?? throw new ArgumentNullException(nameof(webEventDocumentFactory));

            BackgroundWebEventsQueue = backgroundWebEventsQueue
                ?? throw new ArgumentNullException(nameof(backgroundWebEventsQueue));
        }

        public void WriteDocumentAsync(ITrafficLog trafficLog)
        {
            if (trafficLog == null)
                throw new ArgumentNullException(nameof(trafficLog));

            var webEventDocument = WebEventDocumentFactory.Create(trafficLog);

            if (webEventDocument == null)
                throw new NullReferenceException(nameof(webEventDocument));

            BackgroundWebEventsQueue.Queue(IndexEventSplunkContractFactory.Create(
                webEventDocument,
                trafficLog.ApplicationId,
                IndexEventSplunkType.WebEvent));
        }
    }
}