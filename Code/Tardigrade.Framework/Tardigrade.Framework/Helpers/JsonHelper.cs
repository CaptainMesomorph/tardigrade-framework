using System.Text.Json;
using System.Text.Json.Serialization;
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
        /// - DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
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
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            SerializerOptions.Converters.Add(new DateTimeRfc3339JsonConverter());
        }
    }
}