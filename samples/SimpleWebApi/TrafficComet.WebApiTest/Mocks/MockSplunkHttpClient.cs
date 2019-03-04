using Newtonsoft.Json;
using System.Net.Http;
using TrafficComet.Splunk.LogWriter.Abstracts.Containers;
using TrafficComet.Splunk.LogWriter.Abstracts.Http;

namespace TrafficComet.WebApiTest.Mocks
{
    public class MockSplunkHttpClient : SplunkHttpClient
    {
        public MockSplunkHttpClient(ISplunkHttpClientDependenciesContainer splunkHttpClientDependenciesContainer,
            HttpClient httpClient) : base(splunkHttpClientDependenciesContainer, httpClient)
        {
        }

        protected override JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings();

        protected override string SourceName => "MockSplunkHttpClient";

        protected override HttpRequestMessage CreateBaseHttpRequestMessage(HttpMethod httpMethod, string url)
        {
            return new HttpRequestMessage(httpMethod, url);
        }
    }
}