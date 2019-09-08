using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Abstracts.Accessors;
using TrafficComet.Splunk.LogWriter.Abstracts.Extensions;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Readers;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Writers
{
    public abstract class HttpMessageLogWriter<TMessageLogWriter, THttpMessage>
        where TMessageLogWriter : class
        where THttpMessage : class
    {
        protected string ClientId => TrafficCometMiddlewaresAccessor.ClientId;
        protected string TraceId => TrafficCometMiddlewaresAccessor.TraceId;
        protected IHttpContentReader HttpContentReader { get; }
        protected abstract IndexEventSplunkType IndexEventSplunkType { get; }
        protected ILogger<TMessageLogWriter> Logger { get; }
        protected ITrafficCometMiddlewaresAccessor TrafficCometMiddlewaresAccessor { get; }
        protected IWebEventBodyDocumentWriter WebEventBodyDocumentWriter { get; }

        public HttpMessageLogWriter(ILogger<TMessageLogWriter> logger,
            ITrafficCometMiddlewaresAccessor trafficCometMiddlewaresAccessor,
            IWebEventBodyDocumentWriter webEventBodyDocumentWriter,
            IHttpContentReader httpContentReader)
        {
            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            TrafficCometMiddlewaresAccessor = trafficCometMiddlewaresAccessor
                ?? throw new ArgumentNullException(nameof(trafficCometMiddlewaresAccessor));

            WebEventBodyDocumentWriter = webEventBodyDocumentWriter
                ?? throw new ArgumentNullException(nameof(webEventBodyDocumentWriter));

            HttpContentReader = httpContentReader
                ?? throw new ArgumentNullException(nameof(httpContentReader));
        }

        protected abstract string GetFullUrl(THttpMessage httpMessage);

        protected abstract HttpContent GetHttpContent(THttpMessage httpMessage);

        protected async Task<(bool result, dynamic contentValue)> HandleMessageContentAsync(HttpContent httpContent)
        {
            (bool result, dynamic contentValue) = await HttpContentReader.ReadAsync(httpContent);

            if (!result)
            {
                Logger.ContentNotSupported(httpContent.GetType().FullName);
                return (false, default);
            }
            return (result, contentValue);
        }

        protected async ValueTask SaveMessageAsync(THttpMessage httpMessage, string source)
        {
            try
            {
                if (httpMessage == null)
                    throw new ArgumentNullException(nameof(httpMessage));

                if (string.IsNullOrEmpty(source))
                    throw new ArgumentNullException(nameof(source));

                HttpContent httpContent = GetHttpContent(httpMessage);

                if (httpContent != null)
                {
                    (bool result, dynamic contentValue) = await HandleMessageContentAsync(httpContent);

                    if (result)
                    {
                        WebEventBodyDocumentWriter.Write(GetFullUrl(httpMessage), contentValue,
                            source, ClientId, TraceId, IndexEventSplunkType);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}