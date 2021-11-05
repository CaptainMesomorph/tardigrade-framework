using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Tardigrade.Framework.Serialization.Resolvers
{
    /// <summary>
    /// A NewtonSoft JSON contract resolver that does not serialise virtual properties.
    /// Example usage:
    /// <code>
    ///     JsonSerializerSettings settings = new JsonSerializerSettings()
    ///     {
    ///         ContractResolver = new IgnoreVirtualPropertyResolver(),
    ///         Formatting = Formatting.Indented,
    ///         PreserveReferencesHandling = PreserveReferencesHandling.Objects,
    ///         ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    ///     };
    ///     string content = JsonConvert.SerializeObject(obj, settings);
    /// </code>
    /// <a href="https://stackoverflow.com/questions/26162902/how-can-i-do-json-serializer-ignore-navigation-properties">How can I do JSON serializer ignore navigation properties?</a>
    /// </summary>
    public class IgnoreVirtualPropertyResolver : DefaultContractResolver
    {
        /// <summary>
        /// <see cref="DefaultContractResolver.CreateProperty(MemberInfo, MemberSerialization)"/>
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            var propertyInfo = member as PropertyInfo;

            if (propertyInfo != null)
            {
                if (propertyInfo.GetMethod != null &&
                    propertyInfo.GetMethod.IsVirtual &&
                    !propertyInfo.GetMethod.IsFinal)
                {
                    property.ShouldSerialize = _ => false;
                }
            }

            return property;
        }
    }
}