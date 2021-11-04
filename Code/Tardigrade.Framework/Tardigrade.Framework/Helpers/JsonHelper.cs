using System.Text.Json;
using Tardigrade.Framework.Converters;

namespace Tardigrade.Framework.Helpers
{
    /// <summary>
    /// Helper class for JSON related operations.
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// JSON serializer options set to:
        /// - IgnoreNullValues = true
        /// - PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        /// - RFC3339 date/time converter
        /// </summary>
        public static readonly JsonSerializerOptions SerializerOptions;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        static JsonHelper()
        {
            SerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            SerializerOptions.Converters.Add(new DateTimeRfc3339JsonConverter());
        }
    }
}