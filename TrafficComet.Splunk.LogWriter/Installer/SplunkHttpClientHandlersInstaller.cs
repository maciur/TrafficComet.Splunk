using Microsoft.Extensions.DependencyInjection;
using System;
using TrafficComet.Splunk.LogWriter.Http.Handlers;

namespace TrafficComet.Splunk.LogWriter.Installer
{
    public static class SplunkHttpClientHandlersInstaller
    {
        public static IHttpClientBuilder AddSplunkHandlers(this IHttpClientBuilder httpClientBuilder, string source)
        {
            return httpClientBuilder
                .AddSplunkMessageHandler<RequestMessageHandler>(source)
                .AddSplunkMessageHandler<ResponseMessageHandler>(source);
        }

        public static IHttpClientBuilder AddSplunkRequestHandler(this IHttpClientBuilder httpClientBuilder, string source)
        {
            return httpClientBuilder.AddSplunkMessageHandler<RequestMessageHandler>(source);
        }

        public static IHttpClientBuilder AddSplunkResponseHandler(this IHttpClientBuilder httpClientBuilder, string source)
        {
            return httpClientBuilder.AddSplunkMessageHandler<ResponseMessageHandler>(source);
        }

        public static IHttpClientBuilder AddSplunkMessageHandler<TMessageHandler>(this IHttpClientBuilder httpClientBuilder, string source)
            where TMessageHandler : SplunkHttpClientMessageHandler
        {
            if (httpClientBuilder == null)
                throw new ArgumentNullException(nameof(httpClientBuilder));

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException(nameof(source));

            return httpClientBuilder.AddHttpMessageHandler((IServiceProvider service) =>
            {
                var messageHandler = service.GetService<TMessageHandler>();

                if (messageHandler == null)
                    throw new NullReferenceException(nameof(messageHandler));

                messageHandler.Source = source;
                return messageHandler;
            });
        }
    }
}
