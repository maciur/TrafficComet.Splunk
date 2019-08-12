using Newtonsoft.Json;

namespace TrafficComet.Splunk.LogWriter.Documents
{
    public class WebEventDocument : SplunkEventDocument
    {
        [JsonProperty(Order = 0)]
        public string TraceId { get; set; }

        [JsonProperty(Order = 1)]
        public string Source { get; set; }

        [JsonProperty(Order = 2)]
        public WebEventClientDocument Client { get; set; }

        [JsonProperty(Order = 3)]
        public KeyValueDocument[] CustomParams { get; set; }

        [JsonProperty(Order = 4)]
        public WebEventDatesDocument Dates { get; set; }

        [JsonProperty(Order = 5)]
        public WebEventRequestDocument Request { get; set; }

        [JsonProperty(Order = 6)]
        public WebEventResponseDocument Response { get; set; }

        [JsonProperty(Order = 7)]
        public WebEventServerDocument Server { get; set; }
    }
}