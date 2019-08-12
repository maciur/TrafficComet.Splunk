using Microsoft.Extensions.Logging;
using System;
using TrafficComet.Abstracts.Logs;
using TrafficComet.Abstracts.Writers;
using TrafficComet.Splunk.LogWriter.Abstracts.Factories;
using TrafficComet.Splunk.LogWriter.Abstracts.Writers;

namespace TrafficComet.Splunk.LogWriter.Writers
{
    public class TrafficCometSplunkLogWriter : ITrafficLogWriter
	{
		protected IWebEventBodyDocumentWriter WebEventBodyDocumentWriter { get; }
		protected IWebEventDocumentWriter WebEventDocumentWriter { get; }
        protected ILogger<TrafficCometSplunkLogWriter> Logger { get; }

        public TrafficCometSplunkLogWriter(IWebEventBodyDocumentWriter webEventBodyDocumentWriter, 
            IWebEventDocumentWriter webEventDocumentWriter, ILogger<TrafficCometSplunkLogWriter> logger)
		{
			WebEventBodyDocumentWriter = webEventBodyDocumentWriter
				?? throw new ArgumentNullException(nameof(webEventBodyDocumentWriter));

			WebEventDocumentWriter = webEventDocumentWriter
				?? throw new ArgumentNullException(nameof(webEventDocumentWriter));

            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

		public bool SaveLog(ITrafficLog trafficLog)
		{
            try
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
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }

            return false;
		}
	}
}