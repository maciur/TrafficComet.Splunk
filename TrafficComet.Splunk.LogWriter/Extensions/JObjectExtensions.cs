using Newtonsoft.Json.Linq;
using System;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
    internal static class JObjectExtensions
	{
        internal static TOut GetValue<TOut>(this JObject jObject, string path)
		{
			if (jObject == null || !jObject.HasValues)
				throw new ArgumentNullException(nameof(jObject));

			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			var jToken = jObject.SelectToken(path);
			if (jToken != null)
			{
				return jToken.ToObject<TOut>();
			}
			return default(TOut);
		}
	}
}