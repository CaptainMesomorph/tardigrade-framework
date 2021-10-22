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
        /// Default serializer options.
        /// </summary>
        public static readonly JsonSerializerOptions SerializerOptions;

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