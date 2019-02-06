using System;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Queues
{
	public interface IBackgroundSaveLogTasksQueue
	{
		Task<Func<CancellationToken, Task>[]> DequeueAsync(
			CancellationToken cancellationToken, int howMany);

		Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void Queue(Func<CancellationToken, Task> token);
	}
}