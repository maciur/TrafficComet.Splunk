using TrafficSelectors = TrafficComet.Abstracts.Consts.ConfigurationSelectors;

namespace TrafficComet.Splunk.LogWriter.Consts
{
    internal static class ConfigurationSelectors
    {
        #region Base Configuration Paths
        internal readonly static string ROOT = $"{TrafficSelectors.ROOT}:{TrafficSelectors.WRITERS}:Splunk";
        internal readonly static string COLLECTORS = $"{ROOT}:Collectors";
        #endregion

        #region Http Collector
        internal readonly static string HTTP_COLLECTOR = $"{COLLECTORS}:Http";
        internal readonly static string HTTP_COLLECTOR_ENDPOINT = $"{HTTP_COLLECTOR}:EndPoint";
        internal readonly static string HTTP_COLLECTOR_HEALTH_ENDPOINT = $"{HTTP_COLLECTOR}:HealthEndPoint";
        internal readonly static string HTTP_COLLECTOR_INDEX = $"{HTTP_COLLECTOR}:Index";
        internal readonly static string HTTP_COLLECTOR_REQUESTS_INDEX_PREFIX = $"{HTTP_COLLECTOR}:RequestsIndexPrefix";
        internal readonly static string HTTP_COLLECTOR_RESPONSES_INDEX_PREFIX = $"{HTTP_COLLECTOR}:ResponsesIndexPrefix";
        internal readonly static string HTTP_COLLECTOR_TOKEN = $"{HTTP_COLLECTOR}:Token";
        internal readonly static string HTTP_COLLECTOR_URL = $"{HTTP_COLLECTOR}:Url";
        #endregion

        #region Folder Collector 
        internal readonly static string FOLDER_COLLECTOR = $"{COLLECTORS}:Folder";
        internal readonly static string FOLDER_COLLECTOR_PATH = $"{FOLDER_COLLECTOR}:Path";
        internal readonly static string FOLDER_COLLECTOR_REQUESTS_FOLDER = $"{FOLDER_COLLECTOR}:RequestsFolder";
        internal readonly static string FOLDER_COLLECTOR_RESPONSES_FOLDER = $"{FOLDER_COLLECTOR}:ResponsesFolder";
        internal readonly static string FOLDER_COLLECTOR_ROOT_FOLDER = $"{FOLDER_COLLECTOR}:RootFolder";
        #endregion

        #region Hosted Services
        internal readonly static string HOSTED_SERVICES = $"{ROOT}:Services";
        internal readonly static string SERVICES_EVENTS_AT_ONES = $"{HOSTED_SERVICES}:EventsAtOnce";
        #endregion
    }
}
