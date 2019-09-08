using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Extensions;

namespace TrafficComet.Splunk.LogWriter.Http.Handlers
{
    public class ResponseMessageHandler : SplunkHttpClientMessageHandler
    {
        public override string Source { get; set; }
        protected IHttpResponseMessageLogWriter HttpResponseMessageLogWriter { get; }

        public ResponseMessageHandler(IHttpContextAccessor httpContextAccessor,
            IHttpResponseMessageLogWriter httpResponseMessageLogWriter)
            : base(httpContextAccessor)
        {
            HttpResponseMessageLogWriter = httpResponseMessageLogWriter
                ?? throw new ArgumentNullException(nameof(httpResponseMessageLogWriter));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            var responseMessage = await base.SendAsync(request, cancellationToken);

            if (!StopLogging() && !HttpContextAccessor.HttpContext.CheckIfIgnoreResponse(request.RequestUri))
            {
                await HttpResponseMessageLogWriter.SaveAsync(responseMessage, Source);
            }

            return responseMessage;
        }
    }
}