using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with REST API errors.
    /// </summary>
    [Serializable]
    public class RestException : BaseException
    {
        /// <inheritdoc />
        protected RestException() : base()
        {
        }

        /// <inheritdoc />
        protected RestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public RestException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public RestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}