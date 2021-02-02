using Newtonsoft.Json;

namespace Tardigrade.Shared.Tests.Extensions
{
    /// <summary>
    /// This static class contains extension methods for managing JSON operations.
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// Convert the specified object into a JSON formatted string.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="item">Object to convert.</param>
        /// <returns>JSON formatted string representation of the object</returns>
        public static string ToJson<T>(this T item)
        {
            var options = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(item, options);
        }
    }
}