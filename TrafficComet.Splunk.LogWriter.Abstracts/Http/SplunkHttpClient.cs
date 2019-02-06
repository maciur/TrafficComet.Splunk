using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Contents;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Http
{
    public abstract class SplunkHttpClient
    {
        protected abstract string ClientId { get; }
        protected HttpClient HttpClient { get; }
        protected virtual string[] IgnoreHeaders { get; set; }
        protected abstract JsonSerializerSettings JsonSerializerSettings { get; }
        protected abstract string SourceName { get; }
        protected abstract string TraceId { get; }
        protected IWebEventBodyDocumentFactory WebEventBodyDocumentFactory { get; }
        protected IWebEventBodyDocumentWriter WebEventBodyDocumentWriter { get; }

        public SplunkHttpClient(HttpClient httpClient, IWebEventBodyDocumentWriter webEventBodyDocumentWriter,
            IWebEventBodyDocumentFactory webEventBodyDocumentFactory)
        {
            HttpClient = httpClient
                ?? throw new ArgumentNullException(nameof(httpClient));

            WebEventBodyDocumentWriter = webEventBodyDocumentWriter
                ?? throw new ArgumentNullException(nameof(webEventBodyDocumentWriter));

            WebEventBodyDocumentFactory = webEventBodyDocumentFactory
                ?? throw new ArgumentNullException(nameof(webEventBodyDocumentFactory));

            IgnoreHeaders = new string[] { "authorization" };
        }

        public Task<TValue> GetJsonAsync<TValue>(string url, bool ignoreRequest = false)
        {
            return SendJsonAsync<TValue>(CreateBaseHttpRequestMessage(HttpMethod.Get, url), ignoreRequest);
        }

        public Task<TResponse> PostJsonAsync<TResponse>(string url, object requestObject, bool ignoreRequest = false)
        {
            return SendJsonAsync<TResponse>(HttpMethod.Post, url, requestObject, ignoreRequest);
        }

        public Task<TResponse> PutJsonAsync<TResponse>(string url, object requestObject, bool ignoreRequest = false)
        {
            return SendJsonAsync<TResponse>(HttpMethod.Put, url, requestObject, ignoreRequest);
        }

        public Task<TResponse> SendJsonAsync<TResponse>(HttpMethod httpMethod, string url,
            object requestObject, bool ignoreRequest = false)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));

            if (requestObject == null)
                throw new ArgumentNullException(nameof(requestObject));

            var httpRequestMessage = CreateBaseHttpRequestMessage(httpMethod, url);

            if (httpRequestMessage == null)
                throw new NullReferenceException(nameof(httpRequestMessage));

            if (requestObject != null)
            {
                if (!ignoreRequest)
                {
                    WriteDocument(CreateRequestWebEventDocument(httpRequestMessage, requestObject), IndexEventSplunkType.RequestBody);
                }

                httpRequestMessage.Content = new JsonContent(requestObject);
            }

            return SendJsonAsync<TResponse>(httpRequestMessage);
        }

        protected virtual WebEventBodyDocument AddRequestHeaders<THeaders>(WebEventBodyDocument webEventDocument, THeaders headers)
            where THeaders : HttpHeaders
        {
            if (webEventDocument == null)
                throw new ArgumentException(nameof(webEventDocument));

            if (headers != null && headers.Any())
            {
                webEventDocument.CustomParams = headers.Where(x => !IgnoreHeaders.Contains(x.Key.ToLowerInvariant()))
                    .Select(x => new KeyValueDocument
                    {
                        Name = x.Key,
                        Value = x.Value.FirstOrDefault()
                    }).ToArray();
            }

            return webEventDocument;
        }

        protected abstract HttpRequestMessage CreateBaseHttpRequestMessage(HttpMethod httpMethod,
                    string url);

        protected virtual WebEventBodyDocument CreateRequestWebEventDocument(HttpRequestMessage httpRequestMessage, dynamic requestBody)
        {
            if (httpRequestMessage == null)
                throw new ArgumentNullException(nameof(httpRequestMessage));

            var fullUrl = new Uri(HttpClient.BaseAddress, httpRequestMessage.RequestUri);

            return CreateWebEventDocument(requestBody, fullUrl.ToString(), httpRequestMessage.Headers);
        }

        protected virtual WebEventBodyDocument CreateResponseWebEventDocument(HttpResponseMessage httpResponseMessage, dynamic requestBody, string fullUrl)
        {
            if (httpResponseMessage == null)
                throw new ArgumentNullException(nameof(httpResponseMessage));

            WebEventBodyDocument webEventBodyDocument = CreateWebEventDocument(requestBody, fullUrl, httpResponseMessage.Headers);
            webEventBodyDocument.Status = (int)httpResponseMessage.StatusCode;
            return webEventBodyDocument;
        }

        protected virtual WebEventBodyDocument CreateWebEventDocument<THeaders>(dynamic requestBody, string fullUrl, THeaders headers)
            where THeaders : HttpHeaders
        {
            if (requestBody == null)
                throw new ArgumentNullException(nameof(requestBody));

            if (string.IsNullOrEmpty(fullUrl))
                throw new ArgumentNullException(nameof(requestBody));

            WebEventBodyDocument webEventDocument = WebEventBodyDocumentFactory.Create(fullUrl, requestBody, ClientId, TraceId);

            if (webEventDocument == null)
                throw new NullReferenceException(nameof(webEventDocument));

            return AddRequestHeaders(webEventDocument, headers);
        }

        protected async Task<TValue> HandleJsonResponseAsync<TValue>(HttpResponseMessage httpResponseMessage, bool ignoreRequest = false)
        {
            if (httpResponseMessage == null)
                throw new NullReferenceException(nameof(httpResponseMessage));

            var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(stringResponse))
                throw new NullReferenceException(nameof(stringResponse));

            return JsonConvert.DeserializeObject<TValue>(stringResponse, JsonSerializerSettings);
        }

        protected async Task<TResponse> SendJsonAsync<TResponse>(HttpRequestMessage httpRequestMessage, bool ignoreRequest = false)
        {
            if (httpRequestMessage == null)
                throw new ArgumentNullException(nameof(httpRequestMessage));

            var responseHttpMessage = await HttpClient.SendAsync(httpRequestMessage);

            if (responseHttpMessage == null)
                throw new NullReferenceException(nameof(responseHttpMessage));

            var responseObject = await HandleJsonResponseAsync<TResponse>(responseHttpMessage);

            if (!ignoreRequest && responseObject != null)
            {
                WriteDocument(
                    CreateResponseWebEventDocument(responseHttpMessage, responseObject, httpRequestMessage.RequestUri.ToString()),
                    IndexEventSplunkType.ResponseBody);
            }

            return responseObject;
        }

        protected virtual void WriteDocument(WebEventBodyDocument webEventDocument, IndexEventSplunkType indexEventSplunkType)
        {
            if (webEventDocument == null)
                throw new ArgumentNullException(nameof(webEventDocument));

            WebEventBodyDocumentWriter.Write(webEventDocument, SourceName, indexEventSplunkType);
        }
    }
}