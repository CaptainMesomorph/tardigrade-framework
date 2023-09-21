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
        /// <inheritdoc />
        protected EncodingException() : base()
        {
        }

        /// <inheritdoc />
        protected EncodingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public EncodingException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public EncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}