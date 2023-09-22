using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Tardigrade.Framework.Serialization.Xml.Writers;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for XML serialisation.
    /// </summary>
    public static class XmlExtension
    {
        /// <summary>
        /// Serialize the object to an XML string. With this method, the default namespaces will be removed, the XML
        /// declaration will be omitted and boolean values will be replaced with 0's and 1's.
        /// </summary>
        /// <typeparam name="T">Type of the object.</typeparam>
        /// <param name="obj">Object to serialize.</param>
        /// <returns>XML string representing the object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is null.</exception>
        public static string ToXml<T>(this T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            using (var stringWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = new BoolToIntWriter(stringWriter))
                {
                    var serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(xmlWriter, obj, xmlNamespaces);

                    return stringWriter.ToString();
                }
            }
        }
    }
}