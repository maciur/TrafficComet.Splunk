using System;
using TrafficComet.Abstracts.Logs;
using TrafficComet.Abstracts.Logs.Client;
using TrafficComet.Abstracts.Logs.Request;
using TrafficComet.Abstracts.Logs.Response;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Documents;
using TrafficComet.Splunk.LogWriter.Extensions;

namespace TrafficComet.Splunk.LogWriter.Factories
{
	public class WebEventDocumentFactory : IWebEventDocumentFactory
	{
		public WebEventDocument Create(ITrafficLog trafficLog)
		{
			if (trafficLog == null)
				throw new ArgumentNullException(nameof(trafficLog));

			var webEventDocument = new WebEventDocument
			{
				TraceId = trafficLog.TraceId,
				CustomParams = trafficLog.CustomParams.ToDocuments()
			};

			webEventDocument.Client = CreateClientDocument(trafficLog.Client)
				?? throw new NullReferenceException(nameof(webEventDocument.Client));

			webEventDocument.Dates = CreateDatesDocument(trafficLog.Dates)
				?? throw new NullReferenceException(nameof(webEventDocument.Dates));

			webEventDocument.Request = CreateRequestDocument(trafficLog.Request)
				?? throw new NullReferenceException(nameof(webEventDocument.Request));

			webEventDocument.Response = CreateResponseDocument(trafficLog.Response)
				?? throw new NullReferenceException(nameof(webEventDocument.Response));

			webEventDocument.Server = CreateServerDocument(trafficLog.Server)
				?? throw new NullReferenceException(nameof(webEventDocument.Server));

			return webEventDocument;
		}

		protected virtual WebEventClientDocument CreateClientDocument(IClientTrafficLog trafficLogClient)
		{
			if (trafficLogClient == null)
				throw new ArgumentNullException(nameof(trafficLogClient));

			return new WebEventClientDocument
			{
				Id = trafficLogClient.Id,
				IpAddress = trafficLogClient.IpAddress,
				UserAgent = trafficLogClient.UserAgent
			};
		}

		protected virtual WebEventDatesDocument CreateDatesDocument(IDatesTrafficLog trafficLogDates)
		{
			if (trafficLogDates == null)
				throw new ArgumentNullException(nameof(trafficLogDates));

			return new WebEventDatesDocument
			{
				Start = trafficLogDates.Start,
				End = trafficLogDates.End,
				StartUtc = trafficLogDates.StartUtc,
				EndUtc = trafficLogDates.EndUtc
			};
		}

		protected virtual WebEventRequestDocument CreateRequestDocument(IRequestLog trafficLogRequest)
		{
			if (trafficLogRequest == null)
				throw new ArgumentNullException(nameof(trafficLogRequest));

			return new WebEventRequestDocument
			{
				Cookies = trafficLogRequest.Cookies.ToDocuments(),
				CustomParams = trafficLogRequest.CustomParams.ToDocuments(),
				QueryParams = trafficLogRequest.QueryParams.ToDocuments(),
				Headers = trafficLogRequest.Headers.ToDocuments(),
				FullUrl = trafficLogRequest.FullUrl,
				HttpMethod = trafficLogRequest.HttpMethod,
				Path = trafficLogRequest.Path
			};
		}

		protected virtual WebEventResponseDocument CreateResponseDocument(IResponseLog trafficLogResponse)
		{
			if (trafficLogResponse == null)
				throw new ArgumentNullException(nameof(trafficLogResponse));

			return new WebEventResponseDocument
			{
				CustomParams = trafficLogResponse.CustomParams.ToDocuments(),
				Headers = trafficLogResponse.Headers.ToDocuments(),
				Status = trafficLogResponse.Status
			};
		}

		protected virtual WebEventServerDocument CreateServerDocument(IServerTrafficLog trafficLogServer)
		{
			if (trafficLogServer == null)
				throw new ArgumentNullException(nameof(trafficLogServer));

			return new WebEventServerDocument
			{
				IpAddress = trafficLogServer.IpAddress,
				MachineName = trafficLogServer.MachineName
			};
		}
	}
}