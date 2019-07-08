using System.Collections.Generic;
using System.Linq;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
    internal static class ListExtensions
    {
        internal static bool SafeAny<TValue>(this IList<TValue> values)
        {
            return values != null && values.Any();
        }
    }
}
