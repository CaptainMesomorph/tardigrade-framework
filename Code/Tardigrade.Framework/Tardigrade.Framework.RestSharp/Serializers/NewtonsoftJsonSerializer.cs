using Newtonsoft.Json;
using RestSharp.Serializers;
using System.IO;

namespace Tardigrade.Framework.RestSharp.Serializers
{
    /// <summary>
    /// Default JSON serializer for request bodies.
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft attributes.
    /// </summary>
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializer serializer;

        /// <summary>
        /// Default serializer
        /// </summary>
        public NewtonsoftJsonSerializer()
        {
            ContentType = "application/json";

            //serializer = new JsonSerializer
            //{
            //    MissingMemberHandling = MissingMemberHandling.Ignore,
            //    NullValueHandling = NullValueHandling.Include,
            //    DefaultValueHandling = DefaultValueHandling.Include
            //};
            serializer = new JsonSerializer();
        }

        /// <summary>
        /// Default serializer with overload for allowing custom Json.NET settings
        /// </summary>
        public NewtonsoftJsonSerializer(JsonSerializer serializer)
        {
            ContentType = "application/json";
            this.serializer = serializer;
        }

        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON as String</returns>
        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';
                    serializer.Serialize(jsonTextWriter, obj);
                    var result = stringWriter.ToString();

                    return result;
                }
            }
        }
    }
}