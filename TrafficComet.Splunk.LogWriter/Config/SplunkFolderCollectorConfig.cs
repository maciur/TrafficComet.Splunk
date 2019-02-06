namespace TrafficComet.Splunk.LogWriter.Config
{
    public class SplunkFolderCollectorConfig
    {
        public string Path { get; set; }
        public string RootFolder { get; set; }
        public string RequestsFolder { get; set; }
        public string ResponsesFolder { get; set; }
    }
}