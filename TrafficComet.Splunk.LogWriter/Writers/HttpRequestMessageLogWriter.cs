using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Abstracts.Accessors;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Readers;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;

namespace TrafficComet.Splunk.LogWriter.Writers
{
    public class HttpRequestMessageLogWriter : HttpMessageLogWriter<HttpRequestMessageLogWriter, HttpRequestMessage>, IHttpRequestMessageLogWriter
    {
        public HttpRequestMessageLogWriter(ILogger<HttpRequestMessageLogWriter> logger,
            ITrafficCometMiddlewaresAccessor trafficCometMiddlewaresAccessor,
            IWebEventBodyDocumentWriter webEventBodyDocumentWriter,
            IHttpContentReader httpContentReader)
            : base(logger, trafficCometMiddlewaresAccessor, webEventBodyDocumentWriter, httpContentReader)
        {
        }

        protected override IndexEventSplunkType IndexEventSplunkType => IndexEventSplunkType.RequestBody;

        public ValueTask SaveAsync(HttpRequestMessage request, string source)
        {
            return SaveMessageAsync(request, source);
        }

        protected override string GetFullUrl(HttpRequestMessage httpMessage)
        {
            if (httpMessage == null)
                throw new ArgumentNullException(nameof(httpMessage));

            return httpMessage.RequestUri.OriginalString;
        }

        protected override HttpContent GetHttpContent(HttpRequestMessage httpMessage)
        {
            if (httpMessage == null)
                throw new ArgumentNullException(nameof(httpMessage));

            return httpMessage.Content;
        }
    }
}