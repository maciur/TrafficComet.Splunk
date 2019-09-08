using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Readers;
using TrafficComet.Splunk.LogWriter.Documents;
using TrafficComet.Splunk.LogWriter.Enums;

namespace TrafficComet.Splunk.LogWriter.Factories
{
    public class HtttpCollectorResponseSplunkFactory : IHtttpCollectorResponseSplunkFactory
    {
        protected const string CODE_SELECTOR = "code";
        protected const string TEXT_SELECTOR = "text";
        protected const string INVALID_EVENT_NUMBER_SELECTOR = "invalid-event-number";
        protected const int INTERNAL_SERVER_ERROR_CODE = 500;

        protected IHttpContentReader HttpContentReader { get; }

        public HtttpCollectorResponseSplunkFactory(IHttpContentReader httpContentReader)
        {
            HttpContentReader = httpContentReader
                ?? throw new ArgumentNullException(nameof(httpContentReader));
        }

        public async Task<HttpCollectorResponseDocument> CreateFailureResponse(HttpContent httpContent)
        {
            if (httpContent == null)
                throw new ArgumentNullException(nameof(httpContent));

            (bool result, dynamic contentValue) = await HttpContentReader.ReadAsync(httpContent);

            if (result)
            {
                if (!(contentValue is JToken responseAsJToken) || !responseAsJToken.HasValues)
                    throw new NullReferenceException(nameof(responseAsJToken));

                return new HttpCollectorResponseDocument
                {
                    Code = responseAsJToken.Value<int>(CODE_SELECTOR),
                    Text = responseAsJToken.Value<string>(TEXT_SELECTOR),
                    InvalidEventNumber = responseAsJToken.Value<int?>(INVALID_EVENT_NUMBER_SELECTOR)
                };
            }

            return new HttpCollectorResponseDocument
            {
                Code = (int)HttpCollectorResponseStatus.InternalServerError
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