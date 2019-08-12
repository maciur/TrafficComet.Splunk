using System;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Writers
{
    public class WebEventBodyDocumentWriter : IWebEventBodyDocumentWriter
    {
        protected IIndexEventContainerDocumentFactory IndexEventSplunkContractFactory { get; }
        protected IWebEventBodyDocumentFactory WebEventBodyDocumentFactory { get; }
        protected IBackgroundWebEventsQueue BackgroundWebEventsQueue { get; }

        public WebEventBodyDocumentWriter(IIndexEventContainerDocumentFactory indexEventSplunkContractFactory,
            IWebEventBodyDocumentFactory webEventBodyDocumentFactory, IBackgroundWebEventsQueue backgroundWebEventsQueue)
        {
            IndexEventSplunkContractFactory = indexEventSplunkContractFactory
                ?? throw new ArgumentNullException(nameof(indexEventSplunkContractFactory));

            WebEventBodyDocumentFactory = webEventBodyDocumentFactory
                ?? throw new ArgumentNullException(nameof(webEventBodyDocumentFactory));

            BackgroundWebEventsQueue = backgroundWebEventsQueue
                ?? throw new ArgumentNullException(nameof(backgroundWebEventsQueue));
        }

        public void Write(string url, dynamic requestObject, string applicationId, string clientId,
            string traceId, IndexEventSplunkType indexEventSplunkType)
        {
            ValidateParams(url, requestObject, applicationId, clientId, traceId);

            var webEventBodyDocument = WebEventBodyDocumentFactory
                .Create(url, requestObject, clientId, traceId, applicationId);

            Write(webEventBodyDocument, applicationId, indexEventSplunkType);
        }

        public void Write(WebEventBodyDocument webEventBodyDocument, string applicationId, IndexEventSplunkType indexEventSplunkType)
        {
            if (webEventBodyDocument == null)
                throw new ArgumentNullException(nameof(webEventBodyDocument));

            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException(nameof(applicationId));

            BackgroundWebEventsQueue.Queue(IndexEventSplunkContractFactory
                .Create(webEventBodyDocument, applicationId, indexEventSplunkType));
        }

        protected void ValidateParams(string url, dynamic requestObject, string applicationId,
            string clientId, string traceId)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (requestObject == null)
                throw new ArgumentNullException(nameof(requestObject));

            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException(nameof(applicationId));

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            if (string.IsNullOrEmpty(traceId))
                throw new ArgumentNullException(nameof(traceId));
        }
    }
}