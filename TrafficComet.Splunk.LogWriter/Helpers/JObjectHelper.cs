using Newtonsoft.Json.Linq;

namespace TrafficComet.Splunk.LogWriter.Helpers
{
    internal static class JObjectHelper
    {
        internal static bool TryParse(string jsonSource, out JObject jObject)
        {
            jObject = default(JObject);
            if (!string.IsNullOrEmpty(jsonSource))
            {
                try
                {
                    jObject = JObject.Parse(jsonSource);
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
