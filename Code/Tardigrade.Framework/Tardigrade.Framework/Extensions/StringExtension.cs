using System.IO;
using System.Text;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for string operations.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Convert a string into a stream.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <param name="encoding">Character encoding used for the conversion. If not provided, defaults to UTF8.</param>
        /// <returns>Stream representing the string.</returns>
        public static Stream ToStream(this string value, Encoding encoding = null)
            => new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(value ?? string.Empty));
    }
}