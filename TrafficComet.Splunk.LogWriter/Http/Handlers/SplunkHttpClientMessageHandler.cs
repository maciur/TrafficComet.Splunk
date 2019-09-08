using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using TrafficComet.Splunk.LogWriter.Extensions;

namespace TrafficComet.Splunk.LogWriter.Http.Handlers
{
    public abstract class SplunkHttpClientMessageHandler : DelegatingHandler
    {
        protected IHttpContextAccessor HttpContextAccessor { get; }
        public abstract string Source { get; set; }

        public SplunkHttpClientMessageHandler(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor
                ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        protected bool StopLogging()
        {
            if (HttpContextAccessor.HttpContext.TryGetItem("splunk-stop-logging", out bool value)
                && !value)
            {
                return value;
            }
            return false;
        }
    }
}