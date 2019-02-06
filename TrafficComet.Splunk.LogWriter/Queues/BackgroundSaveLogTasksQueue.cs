using TrafficComet.Splunk.LogWriter.Abstracts.Queues;

namespace TrafficComet.Splunk.LogWriter.Queues
{
    public class BackgroundSaveLogTasksQueue : BackgroundTaskQueue, IBackgroundSaveLogTasksQueue
    {
        public BackgroundSaveLogTasksQueue() : base(10)
        {
        }
    }
}