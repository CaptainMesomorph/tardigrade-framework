using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tardigrade.Framework.Converters
{
    /// <summary>
    /// JSON serialisation converter for UTC DateTime that meets the
    /// <a href="https://datatracker.ietf.org/doc/html/rfc3339">RFC3339</a> standard. Specifically, the format applied
    /// is "yyyy-MM-ddTHH:mm:ssK".
    /// </summary>
    public class DateTimeRfc3339JsonConverter : JsonConverter<DateTime>
    {
        /// <inheritdoc />
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? string.Empty).ToUniversalTime();
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            DateTime dateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            writer.WriteStringValue(dateTime.ToString("yyyy-MM-ddTHH:mm:ssK"));
        }
    }
}