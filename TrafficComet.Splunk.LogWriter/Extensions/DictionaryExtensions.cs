using System.Collections.Generic;
using System.Linq;
using TrafficComet.Splunk.LogWriter.Documents;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
	internal static class DictionaryExtensions
	{
		internal static KeyValueDocument[] ToDocuments(this IDictionary<string, string> values)
		{
			if (values != null && values.Any())
			{
				return values.Select(x => new KeyValueDocument(x.Key, x.Value)).ToArray();
			}
			return null;
		}
	}
}