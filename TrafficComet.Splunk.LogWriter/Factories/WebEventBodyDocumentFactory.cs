﻿using System;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Factories
{
    public class WebEventBodyDocumentFactory : IWebEventBodyDocumentFactory
    {
        public WebEventBodyDocument Create(string fullUrl, dynamic requestBody,
            string clientId, string traceId, string applicationId)
        {
            if (string.IsNullOrEmpty(fullUrl))
                throw new ArgumentNullException(nameof(fullUrl));

            if (requestBody == null)
                throw new ArgumentNullException(nameof(requestBody));

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentNullException(nameof(clientId));

            if (string.IsNullOrEmpty(traceId))
                throw new ArgumentNullException(nameof(traceId));

            if (string.IsNullOrEmpty(applicationId))
                throw new ArgumentNullException(nameof(applicationId));

            return new WebEventBodyDocument
            {
                FullUrl = fullUrl,
                ClientId = clientId,
                TraceId = traceId,
                Body = requestBody,
                Source = applicationId
            };
        }
    }
}