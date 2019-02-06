namespace TrafficComet.Splunk.LogWriter.Documents
{
	public class HttpCollectorResponseDocument
	{
		public int Code { get; set; }
		public string Text { get; set; }
		public int? InvalidEventNumber { get; set; }
	}
}