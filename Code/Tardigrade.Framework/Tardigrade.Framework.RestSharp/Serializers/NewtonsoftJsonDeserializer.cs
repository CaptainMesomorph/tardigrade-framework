using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers;
using System;

namespace Tardigrade.Framework.RestSharp.Serializers
{
    /// <summary>
    /// <see cref="IDeserializer"/>
    /// </summary>
    public class NewtonsoftJsonDeserializer : IDeserializer
    {
#if NET
        private static readonly Lazy<NewtonsoftJsonDeserializer> LazyInstance = new();
#else
        private static readonly Lazy<NewtonsoftJsonDeserializer> LazyInstance = new Lazy<NewtonsoftJsonDeserializer>();
#endif

        /// <summary>
        /// Create lazy loaded singleton instance.
        /// </summary>
        public static NewtonsoftJsonDeserializer Instance => LazyInstance.Value;

        /// <summary>
        /// Not used for JSON deserialization.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// <see cref="IDeserializer.Deserialize{T}(RestResponse)"/>
        /// </summary>
        public T Deserialize<T>(RestResponse response)
        {
            return response?.Content == null ? default : JsonConvert.DeserializeObject<T>(response.Content);
        }

        /// <summary>
        /// Not used for JSON deserialization.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Not used for JSON deserialization.
        /// </summary>
        public string RootElement { get; set; }
    }
}