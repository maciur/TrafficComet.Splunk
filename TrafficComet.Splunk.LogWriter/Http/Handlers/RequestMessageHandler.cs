using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Extensions;

namespace TrafficComet.Splunk.LogWriter.Http.Handlers
{
    public class RequestMessageHandler : SplunkHttpClientMessageHandler
    {
        public override string Source { get; set; }
        protected IHttpRequestMessageLogWriter HttpRequestMessageLogWriter { get; }

        public RequestMessageHandler(IHttpContextAccessor httpContextAccessor,
            IHttpRequestMessageLogWriter httpRequestMessageLogWriter)
            : base(httpContextAccessor)
        {
            HttpRequestMessageLogWriter = httpRequestMessageLogWriter
                ?? throw new ArgumentNullException(nameof(httpRequestMessageLogWriter));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Source))
                throw new NullReferenceException(nameof(Source));

            if (!StopLogging() && !HttpContextAccessor.HttpContext.CheckIfIgnoreRequest(request.RequestUri))
            {
                await HttpRequestMessageLogWriter.SaveAsync(request, Source);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}