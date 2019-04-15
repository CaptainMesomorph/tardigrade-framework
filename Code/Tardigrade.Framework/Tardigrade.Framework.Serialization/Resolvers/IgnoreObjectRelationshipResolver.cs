using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Tardigrade.Framework.Serialization.Resolvers
{
    /// <summary>
    /// A NewtonSoft JSON contract resolver that does not serialise object relationships.
    /// Example usage:
    /// <code>
    ///     JsonSerializerSettings settings = new JsonSerializerSettings()
    ///     {
    ///         ContractResolver = new IgnoreObjectRelationshipResolver(),
    ///         Formatting = Formatting.Indented,
    ///         PreserveReferencesHandling = PreserveReferencesHandling.Objects,
    ///         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    ///     };
    ///     string content = JsonConvert.SerializeObject(obj, settings);
    /// </code>
    /// <a href="https://stackoverflow.com/questions/21292010/how-do-i-make-json-net-ignore-object-relationships">How do I make JSON.NET ignore object relationships?</a>
    /// </summary>
    public class IgnoreObjectRelationshipResolver : DefaultContractResolver
    {
        /// <summary>
        /// <see cref="DefaultContractResolver.CreateProperty(MemberInfo, MemberSerialization)"/>
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                property.ShouldSerialize = obj => false;
            }

            return property;
        }
    }
}