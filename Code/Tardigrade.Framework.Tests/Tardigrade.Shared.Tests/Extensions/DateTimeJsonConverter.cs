using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tardigrade.Shared.Tests.Extensions
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            DateTime dateTime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            writer.WriteStringValue(dateTime.ToString("yyyy-MM-ddTHH:mm:ssK"));
        }
    }
}