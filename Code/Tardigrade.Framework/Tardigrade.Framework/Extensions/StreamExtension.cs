using System.IO;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the Stream class.
    /// </summary>
    public static class StreamExtension
    {
        /// <summary>
        /// Convert the stream to a byte array.
        /// </summary>
        /// <param name="stream">Stream to convert.</param>
        /// <returns>Byte array representation of the stream.</returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
            {
                return memoryStream.ToArray();
            }

            using (MemoryStream memoryStreamCopy = new MemoryStream())
            {
                stream.CopyTo(memoryStreamCopy);
                return memoryStreamCopy.ToArray();
            }
        }
    }
}