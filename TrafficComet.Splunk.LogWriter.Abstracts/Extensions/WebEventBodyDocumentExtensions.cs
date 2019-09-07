using System;
using System.Linq;
using System.Net.Http.Headers;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Extensions
{
    internal static class WebEventBodyDocumentExtensions
    {
        internal static WebEventBodyDocument AddRequestHeaders<THeaders>(this WebEventBodyDocument webEventDocument, 
            THeaders headers, string[] ignoreHeaders) where THeaders : HttpHeaders
        {
            if (webEventDocument == null)
                throw new ArgumentException(nameof(webEventDocument));

            if (headers != null && headers.Any())
            {
                webEventDocument.CustomParams = headers.Where(x => !ignoreHeaders.Contains(x.Key.ToLowerInvariant()))
                    .Select(x => new KeyValueDocument
                    {
                        Name = x.Key,
                        Value = x.Value.FirstOrDefault()
                    }).ToArray();
            }

            return webEventDocument;
        }
    }
}
