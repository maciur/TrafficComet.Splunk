using System.Threading;

namespace TrafficComet.Splunk.LogWriter.Accessors
{
    internal class SplunkHealthStatusAccessor
    {
        internal static bool IsHealthy { get; private set; }
        private static SemaphoreSlim Signal { get; }

        static SplunkHealthStatusAccessor()
        {
            Signal = new SemaphoreSlim(1);
        }

        internal static void IsHealthAgain()
        {
            if (!IsHealthy)
            {
                ChangeHealthStatus(true);
            }
        }

        internal static void StartCheckingHealthStatus()
        {
            if (IsHealthy)
            {
                ChangeHealthStatus(false);
            }
        }

        protected static void ChangeHealthStatus(bool status)
        {
            Signal.Wait();
            IsHealthy = status;
            Signal.Release();
        }
    }
}