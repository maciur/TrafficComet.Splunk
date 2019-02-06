namespace TrafficComet.Splunk.LogWriter.Documents
{
    public class WebEventBodyDocument : SplunkEventDocument
    {
        public string TraceId { get; set; }
        public string ClientId { get; set; }
        public dynamic Body { get; set; }
        public string FullUrl { get; set; }
        public int? Status { get; set; }
        public KeyValueDocument[] CustomParams { get; set; }
    }
}