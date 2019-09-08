using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TrafficComet.Splunk.LogWriter.Abstracts.Readers;

namespace TrafficComet.Splunk.LogWriter.Readers
{
    public class JsonHttpContentReader : IHttpContentReader
    {
        protected ILogger<JsonHttpContentReader> Logger { get; }

        public JsonHttpContentReader(ILogger<JsonHttpContentReader> logger)
        {
            Logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool result, dynamic contentValue)> ReadAsync(HttpContent httpContent)
        {
            if (httpContent == null)
                throw new ArgumentNullException(nameof(httpContent));

            try
            {
                var stringMessageValue = await httpContent.ReadAsStringAsync();
                if (IsValidJson(stringMessageValue))
                {
                    return (true, JToken.Parse(stringMessageValue));
                }
            }
            catch (Exception ex)
            {
                Logger.LogDebug(ex, ex.ToString());
            }

            return (false, default);
        }

        protected bool IsValidJson(string stringMessageValue)
        {
            if (!string.IsNullOrEmpty(stringMessageValue))
            {
                if (stringMessageValue[0] == '{' || stringMessageValue[0] == '[')
                {
                    return true;
                }
            }
            return false;
        }
    }
}