using System;
using TrafficComet.Abstracts.Logs;
using TrafficComet.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;

namespace TrafficComet.Splunk.LogWriter.Writers
{
    public class SplunkLogWriter : ITrafficLogWriter
	{
		protected IWebEventBodyDocumentWriter WebEventBodyDocumentWriter { get; }
		protected IWebEventDocumentWriter WebEventDocumentWriter { get; }

		public SplunkLogWriter(IWebEventBodyDocumentWriter webEventBodyDocumentWriter, 
            IWebEventDocumentWriter webEventDocumentWriter)
		{
			WebEventBodyDocumentWriter = webEventBodyDocumentWriter
				?? throw new ArgumentNullException(nameof(webEventBodyDocumentWriter));

			WebEventDocumentWriter = webEventDocumentWriter
				?? throw new ArgumentNullException(nameof(webEventDocumentWriter));
		}

		public bool SaveLog(ITrafficLog trafficLog)
		{
			if (trafficLog == null)
				throw new ArgumentNullException(nameof(trafficLog));

			WebEventDocumentWriter.WriteDocumentAsync(trafficLog);

			if (trafficLog.Request.Body != null)
			{
                WebEventBodyDocumentWriter.Write(
                    trafficLog.Request.FullUrl, trafficLog.Request.Body, trafficLog.ApplicationId,
                    trafficLog.Client.Id, trafficLog.TraceId, IndexEventSplunkType.RequestBody);
			}

			if (trafficLog.Response.Body != null)
			{
                WebEventBodyDocumentWriter.Write(
                    trafficLog.Request.FullUrl, trafficLog.Response.Body, trafficLog.ApplicationId,
                    trafficLog.Client.Id, trafficLog.TraceId, IndexEventSplunkType.ResponseBody);
			}

			return true;
		}
	}
}