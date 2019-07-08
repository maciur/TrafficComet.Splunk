using Microsoft.Extensions.Configuration;
using System;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Factories
{
    public class IndexEventContainerDocumentFactory : IIndexEventContainerDocumentFactory
    {
        protected const string SOURCE_TYPE = "_json";
        protected IConfiguration Configuration { get; }
        protected virtual Func<string, string, string> CreateIndexName { get; set; }

        public IndexEventContainerDocumentFactory(IConfiguration configuration)
        {
            Configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));

            CreateIndexName = (indexName, prefix) =>
            {
                if (string.IsNullOrEmpty(indexName))
                    throw new ArgumentNullException(nameof(indexName));

                if (string.IsNullOrEmpty(prefix))
                    throw new ArgumentNullException(nameof(prefix));

                return $"{indexName}-{prefix}";
            };
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
                Index = GetIndexName(eventSplunkType),
                Time = time,
                SourceType = SOURCE_TYPE,
                Source = source,
                Event = eventDocument
            };
        }

        protected virtual string GetIndexName(IndexEventSplunkType eventSplunkType)
        {
            if (CreateIndexName == null)
                throw new NotImplementedException(nameof(CreateIndexName));

            var indexName = Configuration.GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_INDEX);

            switch (eventSplunkType)
            {
                case IndexEventSplunkType.RequestBody:
                    return CreateIndexName(indexName, Configuration
                        .GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_REQUESTS_INDEX_PREFIX));

                case IndexEventSplunkType.ResponseBody:
                    return CreateIndexName(indexName, Configuration
                         .GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_RESPONSES_INDEX_PREFIX));

                default: return indexName;
            }
        }
    }
}