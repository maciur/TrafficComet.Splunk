using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Exceptions
{
    public class TrafficCometSplunkHttpException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        public string RequestUrl { get; }
        public dynamic Body { get; }

        public TrafficCometSplunkHttpException(HttpStatusCode httpStatusCode, dynamic body,
            string requestUrl = null) : base($"Http Error: {(int)httpStatusCode}")
        {
            Body = body;
            RequestUrl = requestUrl;
            StatusCode = httpStatusCode;
        }

        public static async Task<TrafficCometSplunkHttpException> CreateAsync(HttpRequestMessage httpRequestMessage,
            HttpResponseMessage httpResponseMessage)
        {
            var stringBody = await httpResponseMessage.Content.ReadAsStringAsync();
            dynamic responseBody = null;

            if (!string.IsNullOrEmpty(stringBody))
            {
                try
                {
                    responseBody = JsonConvert.DeserializeObject<JObject>(stringBody);
                }
                catch { }
            }

            return new TrafficCometSplunkHttpException(httpResponseMessage.StatusCode, responseBody,
                httpRequestMessage.RequestUri.ToString());
        }
    }
}
