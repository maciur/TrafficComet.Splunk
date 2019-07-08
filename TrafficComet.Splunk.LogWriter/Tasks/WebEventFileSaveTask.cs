using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Consts;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Tasks
{
    public class WebEventFileSaveTask : WebEventSaveTask
    {
        protected IConfiguration Configuration { get; }
        protected virtual Func<string, string> CreateLogFileName { get; set; }
        protected virtual Encoding Encoding { get; set; }
        protected virtual JsonSerializerSettings JsonSerializerSettings { get; }
        protected virtual Func<IndexEventContainerDocument, JsonSerializerSettings, string> SerializeLog { get; set; }

        public WebEventFileSaveTask(ILogger<WebEventFileSaveTask> logger, IConfiguration configuration)
            : base(logger)
        {
            Configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));

            JsonSerializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            CreateLogFileName = (string sourceName) => $"{Guid.NewGuid()}-{sourceName}.json";
            SerializeLog = (indexEvent, settings) => JsonConvert.SerializeObject(indexEvent, settings);
            Encoding = Encoding.UTF8;
        }

        public bool SaveLog()
        {
            if (WebEvent == null)
                throw new ArgumentNullException(nameof(WebEvent));

            HandleSaveLog(HandleLogPath());

            return true;
        }

        protected string HandleLogPath()
        {
            if (string.IsNullOrEmpty(WebEvent.Source))
                throw new ArgumentNullException(nameof(WebEvent.Source));

            if (CreateLogFileName == null)
                throw new NotImplementedException(nameof(CreateLogFileName));

            var logFolderPath = Configuration.GetValue<string>(ConfigurationSelectors.FOLDER_COLLECTOR_PATH);

            if (string.IsNullOrEmpty(logFolderPath))
                throw new NullReferenceException(nameof(logFolderPath));

            if (!Directory.Exists(logFolderPath))
                throw new DirectoryNotFoundException(nameof(logFolderPath));

            var logFilename = CreateLogFileName(WebEvent.Source);

            if (string.IsNullOrEmpty(logFilename))
                throw new NullReferenceException(nameof(logFilename));

            var subFolderName = GetSubFolderName();

            if (string.IsNullOrEmpty(subFolderName))
                throw new NullReferenceException(nameof(subFolderName));

            return Path.Combine(logFolderPath, subFolderName, logFilename);
        }

        protected void HandleSaveLog(string pathToLogFile)
        {
            if (string.IsNullOrEmpty(pathToLogFile))
                throw new ArgumentNullException(nameof(pathToLogFile));

            if (SerializeLog == null)
                throw new NotImplementedException(nameof(SerializeLog));

            var serializedLog = SerializeLog(WebEvent, JsonSerializerSettings);

            if (string.IsNullOrEmpty(serializedLog))
                throw new NullReferenceException(nameof(serializedLog));

            File.WriteAllText(pathToLogFile, serializedLog, Encoding);
        }

        protected override ValueTask<bool> SaveTaskAsync()
        {
            return new ValueTask<bool>(SaveLog());
        }

        private string GetSubFolderName()
        {
            if (WebEvent.Index.EndsWith(Configuration.GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_REQUESTS_INDEX_PREFIX)))
                return Configuration.GetValue<string>(ConfigurationSelectors.FOLDER_COLLECTOR_REQUESTS_FOLDER);

            if (WebEvent.Index.EndsWith(Configuration.GetValue<string>(ConfigurationSelectors.HTTP_COLLECTOR_RESPONSES_INDEX_PREFIX)))
                return Configuration.GetValue<string>(ConfigurationSelectors.FOLDER_COLLECTOR_RESPONSES_FOLDER);

            return Configuration.GetValue<string>(ConfigurationSelectors.FOLDER_COLLECTOR_ROOT_FOLDER);
        }
    }
}
