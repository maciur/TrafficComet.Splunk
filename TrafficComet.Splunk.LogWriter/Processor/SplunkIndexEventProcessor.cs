using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;
using TrafficComet.Splunk.LogWriter.Abstracts.Processor;
using TrafficComet.Splunk.LogWriter.Abstracts.Queues;
using TrafficComet.Splunk.LogWriter.Accessors;
using TrafficComet.Splunk.LogWriter.Config;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Processor
{
    public class SplunkIndexEventProcessor : ISplunkIndexEventProcessor
    {
        protected IBackgroundSaveLogTasksQueue BackgroundTasks { get; }
        protected IOptions<SplunkFolderCollectorConfig> FolderCollectorConfig { get; }
        protected IOptions<SplunkHttpCollectorConfig> HttpCollectorConfig { get; }

        protected virtual JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        protected ILogger<SplunkIndexEventProcessor> Logger { get; }
        protected string SplunkFolderCollectorPath => FolderCollectorConfig.Value.Path;
        protected ISplunkHttpCollectorClient SplunkHttpCollectorClient { get; }
        protected string SplunkHttpCollectorEndPoint => HttpCollectorConfig.Value.EndPoint;

        public SplunkIndexEventProcessor(IBackgroundSaveLogTasksQueue backgroundSaveLogTasksQueue,
            ISplunkHttpCollectorClient splunkHttpCollectorClient, IOptions<SplunkHttpCollectorConfig> httpCollectorConfig,
            IOptions<SplunkFolderCollectorConfig> folderCollectorConfig, ILogger<SplunkIndexEventProcessor> logger)
        {
            BackgroundTasks = backgroundSaveLogTasksQueue
                ?? throw new ArgumentNullException(nameof(backgroundSaveLogTasksQueue));

            SplunkHttpCollectorClient = splunkHttpCollectorClient
                ?? throw new ArgumentNullException(nameof(splunkHttpCollectorClient));

            if (httpCollectorConfig == null || httpCollectorConfig.Value == null)
                throw new ArgumentNullException(nameof(httpCollectorConfig));

            HttpCollectorConfig = httpCollectorConfig;

            if (folderCollectorConfig == null || folderCollectorConfig.Value == null)
                throw new ArgumentNullException(nameof(folderCollectorConfig));

            FolderCollectorConfig = folderCollectorConfig;

            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ProcessIndexEvent(IndexEventContainerDocument indexEventSplunkContract)
        {
            if (indexEventSplunkContract == null)
                throw new ArgumentNullException(nameof(indexEventSplunkContract));

            BackgroundTasks.Queue(async token =>
            {
                var saveByHttpCollectorResult = await SaveLogByHttpCollector(indexEventSplunkContract);

                if (!saveByHttpCollectorResult)
                {
                    _ = SaveLogByFolderCollector(indexEventSplunkContract);
                }
            });
        }

        protected ValueTask<bool> SaveLogByFolderCollector(IndexEventContainerDocument indexEventSplunkContract)
        {
            string pathToLogFolder = Path.Combine(SplunkFolderCollectorPath, 
                GetLogFolderName(indexEventSplunkContract.Index));

            if (!Directory.Exists(pathToLogFolder))
            {
                Directory.CreateDirectory(pathToLogFolder);
            }

            string pathToLogFile = Path.Combine(pathToLogFolder, $"{Guid.NewGuid()}-{indexEventSplunkContract.Source}.json");

            File.WriteAllText(pathToLogFile,
                JsonConvert.SerializeObject(indexEventSplunkContract.Event, JsonSerializerSettings),
                    Encoding.UTF8);

            return new ValueTask<bool>(true);
        }

        protected ValueTask<bool> SaveLogByHttpCollector(IndexEventContainerDocument indexEventSplunkContract)
        {
            return SplunkHttpCollectorHealthStatusAccessor.IsHealthy ?
                new ValueTask<bool>(SaveLogByHttpCollectorAsync(indexEventSplunkContract))
                : new ValueTask<bool>(false);
        }

        protected async Task<bool> SaveLogByHttpCollectorAsync(IndexEventContainerDocument indexEventSplunkContract)
        {
            bool result = false;

            try
            {
                var httpCollectorResponse = await SplunkHttpCollectorClient.PostJsonAsync(SplunkHttpCollectorEndPoint, indexEventSplunkContract);

                if (httpCollectorResponse == null)
                    throw new NullReferenceException(nameof(httpCollectorResponse));

                result = httpCollectorResponse.Code == 0;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while trying send index event to splunk");
            }

            if (!result)
            {
                SplunkHttpCollectorHealthStatusAccessor.StartCheckingHealthStatus();
            }

            return result;
        }

        private string GetLogFolderName(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                throw new ArgumentNullException(nameof(indexName));

            if (indexName.Contains(HttpCollectorConfig.Value.ResponseIndexPrefix))
                return FolderCollectorConfig.Value.ResponsesFolder;

            if (indexName.Contains(HttpCollectorConfig.Value.RequestsIndexPrefix))
                return FolderCollectorConfig.Value.RequestsFolder;

            return FolderCollectorConfig.Value.RootFolder;
        }
    }
}