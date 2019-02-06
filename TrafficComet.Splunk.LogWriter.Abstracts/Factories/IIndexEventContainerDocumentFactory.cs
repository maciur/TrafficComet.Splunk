using System;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Factories
{
    public interface IIndexEventContainerDocumentFactory
    {
        IndexEventContainerDocument Create<TEventDocument>(TEventDocument eventDocument,
            string source, IndexEventSplunkType eventSplunkType, DateTime? time = null)
            where TEventDocument : SplunkEventDocument;
    }
}