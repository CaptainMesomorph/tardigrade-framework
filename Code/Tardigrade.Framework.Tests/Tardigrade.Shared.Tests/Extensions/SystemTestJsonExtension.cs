using System;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Tardigrade.Shared.Tests.Extensions
{
    /// <summary>
    /// This static class contains extension methods for JSON serialisation.
    /// </summary>
    public static class SystemTestJsonExtension
    {
        public static readonly JsonSerializerOptions SerializerOptions;

        static SystemTestJsonExtension()
        {
            SerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            SerializerOptions.Converters.Add(new DateTimeJsonConverter());
        }

        /// <summary>
        /// Serialize the credential claim to a JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the claim associated with the subject of the credential.</typeparam>
        /// <param name="claim">Credential claim.</param>
        /// <returns>JSON string representing the credential claim.</returns>
        /// <exception cref="ArgumentNullException">claim is null.</exception>
        /// <exception cref="SerializationException">Unable to serialize the the object.</exception>
        public static string ToSystemTextJson<T>(this T claim)
        {
            if (claim == null) throw new ArgumentNullException(nameof(claim));

            try
            {
                return JsonSerializer.Serialize(claim, SerializerOptions);
            }
            catch (NotSupportedException e)
            {
                throw new SerializationException(
                    $"Unable to serialize the an object -> {e.GetBaseException().Message}", e);
            }
        }
    }
}