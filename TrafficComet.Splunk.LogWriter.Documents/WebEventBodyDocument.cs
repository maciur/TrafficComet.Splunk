using Newtonsoft.Json;

namespace TrafficComet.Splunk.LogWriter.Documents
{
    public class WebEventBodyDocument : SplunkEventDocument
    {
        [JsonProperty(Order = 0)]
        public string TraceId { get; set; }

        [JsonProperty(Order = 1)]
        public string Source { get; set; }

        [JsonProperty(Order = 2)]
        public string ClientId { get; set; }

        [JsonProperty(Order = 3)]
        public dynamic Body { get; set; }

        [JsonProperty(Order = 4)]
        public string FullUrl { get; set; }

        [JsonProperty(Order = 5)]
        public int? Status { get; set; }

        [JsonProperty(Order = 6)]
        public KeyValueDocument[] CustomParams { get; set; }
    }
}