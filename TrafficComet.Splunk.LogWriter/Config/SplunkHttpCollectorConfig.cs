namespace TrafficComet.Splunk.LogWriter.Config
{
    public class SplunkHttpCollectorConfig
	{
		public string Url { get; set; }
		public string EndPoint { get; set; }
		public string HealthEndPoint { get; set; }
		public string RequestsIndexPrefix { get; set; }
		public string ResponseIndexPrefix { get; set; }
		public string Token { get; set; }
		public string Index { get; set; }
	}
}