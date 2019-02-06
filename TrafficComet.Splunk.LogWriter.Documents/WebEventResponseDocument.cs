namespace TrafficComet.Splunk.LogWriter.Documents
{
    public class WebEventResponseDocument : SplunkEventDocument
	{
		public KeyValueDocument[] CustomParams { get; set; }
		public KeyValueDocument[] Headers { get; set; }
		public int Status { get; set; }
	}
}