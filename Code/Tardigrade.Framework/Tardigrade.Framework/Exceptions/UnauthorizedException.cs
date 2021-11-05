using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with a HTTP status code of 401 (Unauthorized).
    /// </summary>
    [Serializable]
    public class UnauthorizedException : BaseException
    {
        /// <inheritdoc />
        protected UnauthorizedException()
        {
        }

        /// <inheritdoc />
        protected UnauthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public UnauthorizedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}