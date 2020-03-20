using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with encoding errors.
    /// </summary>
    [Serializable]
    public class EncodingException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException()"/>
        /// </summary>
        protected EncodingException() : base()
        {
        }

        /// <summary>
        /// <see cref="BaseException(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected EncodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public EncodingException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public EncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}