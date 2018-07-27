using Newtonsoft.Json;
using RestSharp.Serializers;
using System.IO;

namespace Tardigrade.Framework.RestSharp.Serializers
{
    /// <summary>
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly Newtonsoft.Json.JsonSerializer serializer;

        /// <summary>
        /// Default serializer
        /// </summary>
        public NewtonsoftJsonSerializer()
        {
            ContentType = "application/json";

            serializer = new Newtonsoft.Json.JsonSerializer
            {
                //MissingMemberHandling = MissingMemberHandling.Ignore,
                //NullValueHandling = NullValueHandling.Include,
                //DefaultValueHandling = DefaultValueHandling.Include
            };
        }

        /// <summary>
        /// Default serializer with overload for allowing custom Json.NET settings
        /// </summary>
        public NewtonsoftJsonSerializer(Newtonsoft.Json.JsonSerializer serializer)
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
            using (StringWriter stringWriter = new StringWriter())
            {
                using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    jsonTextWriter.Formatting = Formatting.Indented;
                    jsonTextWriter.QuoteChar = '"';
                    serializer.Serialize(jsonTextWriter, obj);
                    string result = stringWriter.ToString();

                    return result;
                }
            }
        }
    }
}