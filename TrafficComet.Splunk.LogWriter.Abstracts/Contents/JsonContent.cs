using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace TrafficComet.Splunk.LogWriter.Abstracts.Contents
{
	public class JsonContent : StringContent
	{
		public JsonContent(object value)
			: base(SerializeObject(value), Encoding.UTF8,
			"application/json")
		{
		}

		public JsonContent(object value, string mediaType)
			: base(SerializeObject(value), Encoding.UTF8, mediaType)
		{
		}

		private static string SerializeObject(object value)
		{
			return JsonConvert.SerializeObject(value, new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			});
		}
	}
}