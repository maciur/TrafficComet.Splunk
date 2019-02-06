using Microsoft.Extensions.Options;
using System;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Config;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Factories
{
    public class IndexEventContainerDocumentFactory : IIndexEventContainerDocumentFactory
    {
        protected const string SOURCE_TYPE = "_json";

        protected IOptions<SplunkHttpCollectorConfig> HttpCollectorConfig { get; }

        public IndexEventContainerDocumentFactory(IOptions<SplunkHttpCollectorConfig> httpCollectorConfig)
        {
            if (httpCollectorConfig == null || httpCollectorConfig.Value == null)
                throw new ArgumentNullException(nameof(httpCollectorConfig));

            HttpCollectorConfig = httpCollectorConfig;
        }

        public IndexEventContainerDocument Create<TEventDocument>(TEventDocument eventDocument,
            string source, IndexEventSplunkType eventSplunkType,
            DateTime? time = null)
            where TEventDocument : SplunkEventDocument
        {
            if (eventDocument == null)
                throw new ArgumentNullException(nameof(eventDocument));

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            return new IndexEventContainerDocument
            {
                Index = GetIndexName(HttpCollectorConfig.Value.Index, eventSplunkType),
                Time = time,
                SourceType = SOURCE_TYPE,
                Source = source,
                Event = eventDocument
            };
        }

        protected virtual string GetIndexName(string indexName, IndexEventSplunkType eventSplunkType)
        {
            if (string.IsNullOrEmpty(indexName))
                throw new ArgumentNullException(nameof(indexName));

            switch (eventSplunkType)
            {
                case IndexEventSplunkType.RequestBody:
                    return $"{indexName}-{HttpCollectorConfig.Value.RequestsIndexPrefix}";

                case IndexEventSplunkType.ResponseBody:
                    return $"{indexName}-{HttpCollectorConfig.Value.ResponseIndexPrefix}";

                default:
                    return indexName;
            }
        }
    }
}