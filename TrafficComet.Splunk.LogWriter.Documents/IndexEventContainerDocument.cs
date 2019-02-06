using Newtonsoft.Json;
using System;

namespace TrafficComet.Splunk.LogWriter.Documents
{
	public class IndexEventContainerDocument
    {
		[JsonProperty("event")]
		public SplunkEventDocument Event { get; set; }

		[JsonProperty("index")]
		public string Index { get; set; }

		[JsonProperty("source")]
		public string Source { get; set; }

		[JsonProperty("sourcetype")]
		public string SourceType { get; set; }

		[JsonProperty("time")]
		public DateTime? Time { get; set; }
	}
}