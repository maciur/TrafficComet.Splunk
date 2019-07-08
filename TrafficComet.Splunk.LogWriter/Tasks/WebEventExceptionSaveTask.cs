using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Exceptions;

namespace TrafficComet.Splunk.LogWriter.Tasks
{
    public class WebEventExceptionSaveTask : WebEventSaveTask
    {
        public WebEventExceptionSaveTask(ILogger<WebEventExceptionSaveTask> logger)
            : base(logger)
        {
        }

        protected override ValueTask<bool> SaveTaskAsync()
        {
            var exception = new TrafficCometFailedSaveWebEventException(WebEvent);
            Logger.LogError(exception, exception.Message);
            return new ValueTask<bool>(true);
        }
    }
}