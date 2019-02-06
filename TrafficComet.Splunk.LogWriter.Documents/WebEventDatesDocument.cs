using System;

namespace TrafficComet.Splunk.LogWriter.Documents
{
	public class WebEventDatesDocument
	{
		public DateTime StartUtc { get; set; }
		public DateTime EndUtc { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}