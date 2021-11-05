using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with failed email operations.
    /// </summary>
    [Serializable]
    public class EmailFailedException : BaseException
    {
        /// <inheritdoc />
        protected EmailFailedException()
        {
        }

        /// <inheritdoc />
        protected EmailFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public EmailFailedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public EmailFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}