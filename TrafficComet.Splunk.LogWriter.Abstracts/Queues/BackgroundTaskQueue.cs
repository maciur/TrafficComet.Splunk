using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Queues
{
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.1
    public abstract class BackgroundTaskQueue
    {
        protected SemaphoreSlim Signal { get; }
        protected ConcurrentQueue<Func<CancellationToken, Task>> WorkItems { get; }

        public BackgroundTaskQueue(int initialCountForSemaphoreSlim)
        {
            WorkItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
            Signal = new SemaphoreSlim(initialCountForSemaphoreSlim);
        }

        public async Task<Func<CancellationToken, Task>[]> DequeueAsync(CancellationToken cancellationToken, int howMany)
        {
            await Signal.WaitAsync(cancellationToken);

            Func<CancellationToken, Task>[] workItems = new Func<CancellationToken, Task>
                [WorkItems.Count > howMany ? howMany : WorkItems.Count];

            for (int i = 0; i < workItems.Length; i++)
            {
                WorkItems.TryDequeue(out var workItem);
                workItems[i] = workItem;
            }

            return workItems;
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await Signal.WaitAsync(cancellationToken);
            WorkItems.TryDequeue(out var workItem);
            return workItem;
        }

        public void Queue(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            WorkItems.Enqueue(workItem);
            Signal.Release();
        }
    }
}