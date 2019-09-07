using System;
using TrafficComet.Splunk.LogWriter.Abstracts.Containers;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Extensions
{
    internal static class DependenciesContainerExtensions
    {
        internal static void WriteDocument(this ISplunkHttpClientDependenciesContainer dependencies,
            WebEventBodyDocument webEventDocument, IndexEventSplunkType indexEventSplunkType, string sourceName)
        {
            if (webEventDocument == null)
                throw new ArgumentNullException(nameof(webEventDocument));

            dependencies.WebEventBodyDocumentWriter.Write(webEventDocument, sourceName, indexEventSplunkType);
        }
    }
}
