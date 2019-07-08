using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Documents;
using TrafficComet.Splunk.LogWriter.Enums;

namespace TrafficComet.Splunk.LogWriter.Tasks
{
    public abstract class WebEventSaveTask
    {
        private const int _maxAttempts = 3;
        public int Attempts { get; private set; }
        public IndexEventContainerDocument WebEvent { get; private set; }
        public bool ReadyToConvert => Attempts == _maxAttempts;
        public WebEventSaveTaskStatus Status { get; private set; }

        protected ILogger<WebEventSaveTask> Logger { get; }

        public WebEventSaveTask(ILogger<WebEventSaveTask> logger)
        {
            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            Status = WebEventSaveTaskStatus.NotRun;
        }

        public ValueTask<WebEventSaveTask> ExecuteAsync()
        {
            Status = WebEventSaveTaskStatus.Started;
            if (WebEvent != null)
            {
                return new ValueTask<WebEventSaveTask>(ExecuteTaskAsync());
            }
            Status = WebEventSaveTaskStatus.Ignore;
            return new ValueTask<WebEventSaveTask>(this);
        }

        public void LoadIndexEvent(IndexEventContainerDocument indexEvent)
        {
            if (indexEvent != null)
            {
                WebEvent = indexEvent;
            }
            else
            {
                Status = WebEventSaveTaskStatus.Ignore;
            }
        }

        protected void DoNotTryAgain()
        {
            Attempts = _maxAttempts;
        }

        protected async Task<WebEventSaveTask> ExecuteTaskAsync()
        {
            try
            {
                Status = (await SaveTaskAsync()) ?
                    WebEventSaveTaskStatus.Ok : WebEventSaveTaskStatus.Failed;
            }
            catch (TaskCanceledException ex)
            {
                CatchDoNotTryAgainBlock(ex);
            }
            catch (OperationCanceledException ex)
            {
                CatchDoNotTryAgainBlock(ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                CatchDoNotTryAgainBlock(ex);
            }
            catch (Exception ex)
            {
                Status = WebEventSaveTaskStatus.Error;
                Logger.LogError(ex, ex.ToString());
            }

            IncreaseAttempts();

            return this;
        }

        protected abstract ValueTask<bool> SaveTaskAsync();

        private void IncreaseAttempts()
        {
            if (Attempts < _maxAttempts)
            {
                Attempts += Status == (WebEventSaveTaskStatus.Ok | WebEventSaveTaskStatus.Ignore) ? 0 : 1;
            }
        }

        private void CatchDoNotTryAgainBlock<TException>(TException ex)
            where TException : Exception
        {
            DoNotTryAgain();
            Status = WebEventSaveTaskStatus.Error;
            Logger.LogError(ex, ex.ToString());
        }
    }
}