using System;
using System.Runtime.Serialization;
using System.Text.Json;
using Tardigrade.Framework.Helpers;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for JSON serialisation.
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// Serialize the object to a JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">Object to serialise.</param>
        /// <param name="options">Options to control serialisation behaviour.</param>
        /// <returns>JSON string representing the object.</returns>
        /// <exception cref="ArgumentNullException">obj is null.</exception>
        /// <exception cref="SerializationException">Unable to serialize the object.</exception>
        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            try
            {
                return JsonSerializer.Serialize(obj, options ?? JsonHelper.SerializerOptions);
            }
            catch (NotSupportedException e)
            {
                throw new SerializationException($"Unable to serialize object: {e.Message}", e);
            }
        }
    }
}