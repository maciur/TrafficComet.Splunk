using System;
using TrafficComet.Abstracts.Logs;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Processor;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;

namespace TrafficComet.Splunk.LogWriter.Writers
{
    public class WebEventDocumentWriter : IWebEventDocumentWriter
    {
        protected IIndexEventContainerDocumentFactory IndexEventSplunkContractFactory { get; }
        protected IWebEventDocumentFactory WebEventDocumentFactory { get; }
        protected ISplunkIndexEventProcessor SplunkIndexEventProcessor { get; }

        public WebEventDocumentWriter(IIndexEventContainerDocumentFactory indexEventSplunkContractFactory,
            IWebEventDocumentFactory webEventDocumentFactory, ISplunkIndexEventProcessor splunkIndexEventProcessor)
        {
            IndexEventSplunkContractFactory = indexEventSplunkContractFactory
                ?? throw new ArgumentNullException(nameof(indexEventSplunkContractFactory));

            WebEventDocumentFactory = webEventDocumentFactory
                ?? throw new ArgumentNullException(nameof(webEventDocumentFactory));

            SplunkIndexEventProcessor = splunkIndexEventProcessor
                ?? throw new ArgumentNullException(nameof(splunkIndexEventProcessor));
        }

        public void WriteDocumentAsync(ITrafficLog trafficLog)
        {
            if (trafficLog == null)
                throw new ArgumentNullException(nameof(trafficLog));

            var webEventDocument = WebEventDocumentFactory.Create(trafficLog);

            if (webEventDocument == null)
                throw new NullReferenceException(nameof(webEventDocument));

            SplunkIndexEventProcessor.ProcessIndexEvent(IndexEventSplunkContractFactory.Create(
                webEventDocument,
                trafficLog.ApplicationId,
                IndexEventSplunkType.WebEvent));
        }
    }
}