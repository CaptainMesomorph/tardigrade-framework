using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with a HTTP status code of 400 (Bad Request).
    /// </summary>
    [Serializable]
    public class BadRequestException : BaseException
    {
        /// <inheritdoc />
        protected BadRequestException()
        {
        }

        /// <inheritdoc />
        protected BadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}