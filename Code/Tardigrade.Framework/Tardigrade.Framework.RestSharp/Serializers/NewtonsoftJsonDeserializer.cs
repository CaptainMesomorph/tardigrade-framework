using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using System;

namespace Tardigrade.Framework.RestSharp.Serializers
{
    /// <summary>
    /// <see cref="IDeserializer"/>
    /// </summary>
    public class NewtonsoftJsonDeserializer : IDeserializer
    {
        private static readonly Lazy<NewtonsoftJsonDeserializer> lazyInstance = new Lazy<NewtonsoftJsonDeserializer>();

        /// <summary>
        /// Create lazy loaded singleton instance.
        /// </summary>
        public static NewtonsoftJsonDeserializer Instance
        {
            get { return lazyInstance.Value; }
        }

        /// <summary>
        /// Not used for JSON deserialisation.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// <see cref="IDeserializer.Deserialize{T}(IRestResponse)"/>
        /// </summary>
        public T Deserialize<T>(IRestResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        /// <summary>
        /// Not used for JSON deserialisation.
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Not used for JSON deserialisation.
        /// </summary>
        public string RootElement { get; set; }
    }
}