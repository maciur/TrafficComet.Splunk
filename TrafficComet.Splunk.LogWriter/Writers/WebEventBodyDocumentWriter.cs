using System;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Processor;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Writers
{
    public class WebEventBodyDocumentWriter : IWebEventBodyDocumentWriter
    {
        protected IIndexEventContainerDocumentFactory IndexEventSplunkContractFactory { get; }
        protected IWebEventBodyDocumentFactory WebEventBodyDocumentFactory { get; }
        protected ISplunkIndexEventProcessor SplunkIndexEventProcessor { get; }

        public WebEventBodyDocumentWriter(IIndexEventContainerDocumentFactory indexEventSplunkContractFactory,
            IWebEventBodyDocumentFactory webEventBodyDocumentFactory, ISplunkIndexEventProcessor splunkIndexEventProcessor)
        {
            IndexEventSplunkContractFactory = indexEventSplunkContractFactory
                ?? throw new ArgumentNullException(nameof(indexEventSplunkContractFactory));

            WebEventBodyDocumentFactory = webEventBodyDocumentFactory
                ?? throw new ArgumentNullException(nameof(webEventBodyDocumentFactory));

            SplunkIndexEventProcessor = splunkIndexEventProcessor
                ?? throw new ArgumentNullException(nameof(splunkIndexEventProcessor));
        }

        public void Write(string url, dynamic requestObject, string sourceName, string clientId,
            string traceId, IndexEventSplunkType indexEventSplunkType)
        {
            ValidateParams(url, requestObject, sourceName, clientId, traceId);

            var webEventBodyDocument = WebEventBodyDocumentFactory
                .Create(url, requestObject, clientId, traceId);

            Write(webEventBodyDocument, sourceName, indexEventSplunkType);
        }

        public void Write(WebEventBodyDocument webEventBodyDocument, string sourceName, IndexEventSplunkType indexEventSplunkType)
        {
            if (webEventBodyDocument == null)
                throw new ArgumentNullException(nameof(webEventBodyDocument));

            if (string.IsNullOrEmpty(sourceName))
                throw new ArgumentNullException(nameof(sourceName));

            SplunkIndexEventProcessor.ProcessIndexEvent(IndexEventSplunkContractFactory
                .Create(webEventBodyDocument, sourceName, indexEventSplunkType));
        }

        protected void ValidateParams(string url, dynamic requestObject, string sourceName,
            string clientId, string traceId)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (requestObject == null)
                throw new ArgumentNullException(nameof(requestObject));

            if (string.IsNullOrEmpty(sourceName))
                throw new ArgumentNullException(nameof(sourceName));

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            if (string.IsNullOrEmpty(traceId))
                throw new ArgumentNullException(nameof(traceId));
        }
    }
}