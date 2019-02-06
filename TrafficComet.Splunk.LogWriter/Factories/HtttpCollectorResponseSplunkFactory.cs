using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Documents;
using TrafficComet.Splunk.LogWriter.Extensions;

namespace TrafficComet.Splunk.LogWriter.Factories
{
    public class HtttpCollectorResponseSplunkFactory : IHtttpCollectorResponseSplunkFactory
    {
        protected const string CODE_SELECTOR = "code";
        protected const string TEXT_SELECTOR = "text";
        protected const string INVALID_EVENT_NUMBER_SELECTOR = "invalid-event-number";
        protected const int INTERNAL_SERVER_ERROR_CODE = 500;

        public async Task<HttpCollectorResponseDocument> CreateFailureResponse(HttpContent httpContent)
        {
            if (httpContent == null)
                throw new ArgumentNullException(nameof(httpContent));

            var stringResponse = await httpContent.ReadAsStringAsync();

            if (!string.IsNullOrEmpty(stringResponse))
            {
                var responseAsJObject = JObject.Parse(stringResponse);

                if (responseAsJObject == null || !responseAsJObject.HasValues)
                    throw new NullReferenceException(nameof(responseAsJObject));

                return new HttpCollectorResponseDocument
                {
                    Code = responseAsJObject.GetValue<int>(CODE_SELECTOR),
                    Text = responseAsJObject.GetValue<string>(TEXT_SELECTOR),
                    InvalidEventNumber = responseAsJObject.GetValue<int?>(INVALID_EVENT_NUMBER_SELECTOR)
                };
            }

            return new HttpCollectorResponseDocument
            {
                Code = INTERNAL_SERVER_ERROR_CODE
            };
        }

        public async Task<HttpCollectorResponseDocument> CreateSuccessResponse(HttpContent httpContent)
        {
            if (httpContent == null)
                throw new ArgumentNullException(nameof(httpContent));

            var stringResponse = await httpContent.ReadAsStringAsync();

            if (string.IsNullOrEmpty(stringResponse))
                throw new NullReferenceException(nameof(stringResponse));

            return JsonConvert.DeserializeObject<HttpCollectorResponseDocument>(stringResponse);
        }
    }
}