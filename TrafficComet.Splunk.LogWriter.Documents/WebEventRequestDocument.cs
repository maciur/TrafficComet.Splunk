namespace TrafficComet.Splunk.LogWriter.Documents
{
    public class WebEventRequestDocument : SplunkEventDocument
	{
		public KeyValueDocument[] Cookies { get; set; }
		public KeyValueDocument[] CustomParams { get; set; }
		public string FullUrl { get; set; }
		public KeyValueDocument[] Headers { get; set; }
		public string HttpMethod { get; set; }
		public string Path { get; set; }
		public KeyValueDocument[] QueryParams { get; set; }
	}
}