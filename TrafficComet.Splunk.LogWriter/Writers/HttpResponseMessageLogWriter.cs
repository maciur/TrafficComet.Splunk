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
    public class HttpResponseMessageLogWriter : HttpMessageLogWriter<HttpResponseMessageLogWriter, HttpResponseMessage>, IHttpResponseMessageLogWriter
    {
        protected override IndexEventSplunkType IndexEventSplunkType => IndexEventSplunkType.ResponseBody;

        public HttpResponseMessageLogWriter(ILogger<HttpResponseMessageLogWriter> logger, 
            ITrafficCometMiddlewaresAccessor trafficCometMiddlewaresAccessor,
            IWebEventBodyDocumentWriter webEventBodyDocumentWriter,
            IHttpContentReader httpContentReader) 
            : base(logger, trafficCometMiddlewaresAccessor, webEventBodyDocumentWriter, httpContentReader)
        {
        }

        public ValueTask SaveAsync(HttpResponseMessage response, string source)
        {
            return SaveMessageAsync(response, source);
        }

        protected override string GetFullUrl(HttpResponseMessage httpMessage)
        {
            if (httpMessage == null)
                throw new ArgumentNullException(nameof(httpMessage));

            return httpMessage.RequestMessage.RequestUri.OriginalString;
        }

        protected override HttpContent GetHttpContent(HttpResponseMessage httpMessage)
        {
            if (httpMessage == null)
                throw new ArgumentNullException(nameof(httpMessage));

            return httpMessage.Content;
        }
    }
}