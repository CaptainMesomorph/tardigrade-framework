using System;
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
        /// Convert a string from Base64.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <param name="encoding">Character encoding used for the conversion. If not provided, defaults to UTF8.</param>
        /// <returns>String that was converted from Base64; null if the original string was null.</returns>
        public static string FromBase64(this string value, Encoding encoding = null)
            => value == null ? null : (encoding ?? Encoding.UTF8).GetString(Convert.FromBase64String(value));

        /// <summary>
        /// Convert a string to Base64.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <param name="encoding">Character encoding used for the conversion. If not provided, defaults to UTF8.</param>
        /// <returns>Base64 string; null if the original string was null.</returns>
        public static string ToBase64(this string value, Encoding encoding = null)
            => value == null ? null : Convert.ToBase64String((encoding ?? Encoding.UTF8).GetBytes(value));

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