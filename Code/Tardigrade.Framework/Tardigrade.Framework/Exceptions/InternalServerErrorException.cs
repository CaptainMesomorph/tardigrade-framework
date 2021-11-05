using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with a HTTP status code of 500 (Internal Server Error).
    /// </summary>
    [Serializable]
    public class InternalServerErrorException : BaseException
    {
        /// <inheritdoc />
        protected InternalServerErrorException()
        {
        }

        /// <inheritdoc />
        protected InternalServerErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public InternalServerErrorException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public InternalServerErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}