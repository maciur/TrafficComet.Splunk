namespace TrafficComet.Splunk.LogWriter.Documents
{
    public class WebEventDocument : SplunkEventDocument
	{
		public WebEventClientDocument Client { get; set; }
		public KeyValueDocument[] CustomParams { get; set; }
		public WebEventDatesDocument Dates { get; set; }
		public WebEventRequestDocument Request { get; set; }
		public WebEventResponseDocument Response { get; set; }
		public WebEventServerDocument Server { get; set; }
		public string TraceId { get; set; }
	}
}