using Microsoft.AspNetCore.Http;
using System;

namespace TrafficComet.Splunk.LogWriter.Extensions
{
    public static class HttpContextExtensions
    {
        private const string REQUEST_KEY_PATTERN = "request_{0}";
        private const string RESPONSE_KEY_PATTERN = "response_{0}";

        public static void IgnoreRequest(this HttpContext httpContext, Uri uri)
        {
            httpContext.AddToIgnore(uri, REQUEST_KEY_PATTERN);
        }

        public static void IgnoreResponse(this HttpContext httpContext, Uri uri)
        {
            httpContext.AddToIgnore(uri, RESPONSE_KEY_PATTERN);
        }

        internal static bool CheckIfIgnoreRequest(this HttpContext httpContext, Uri uri)
        {
            return httpContext.Ignore(uri, REQUEST_KEY_PATTERN);
        }

        internal static bool CheckIfIgnoreResponse(this HttpContext httpContext, Uri uri)
        {
            return httpContext.Ignore(uri, RESPONSE_KEY_PATTERN);
        }

        internal static bool TryGetItem<TValue>(this HttpContext httpContext, string itemKey, out TValue value)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (string.IsNullOrEmpty(itemKey))
                throw new ArgumentNullException(nameof(itemKey));

            value = default;

            if (httpContext.Items.TryGetValue(itemKey, out object item)
                && item is TValue itemValue)
            {
                value = itemValue;
                return true;
            }
            return false;
        }

        private static void AddToIgnore(this HttpContext httpContext, Uri uri, string keyPattern)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (string.IsNullOrEmpty(keyPattern))
                throw new ArgumentNullException(nameof(keyPattern));

            var itemsKey = string.Format(keyPattern, uri.OriginalString);

            if (!httpContext.Items.ContainsKey(itemsKey))
            {
                httpContext.Items.Add(itemsKey, true);
            }
        }

        private static bool Ignore(this HttpContext httpContext, Uri uri, string keyPattern)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (string.IsNullOrEmpty(keyPattern))
                throw new ArgumentNullException(nameof(keyPattern));

            var itemsKey = string.Format(keyPattern, uri.OriginalString);

            if (httpContext.Items.TryGetValue(itemsKey, out object value)
                && value is bool ignore)
            {
                return ignore;
            }
            return false;
        }
    }
}