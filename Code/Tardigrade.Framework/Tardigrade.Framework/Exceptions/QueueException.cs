using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with message queue errors.
    /// </summary>
    [Serializable]
    public class QueueException : BaseException
    {
        /// <inheritdoc />
        protected QueueException()
        {
        }

        /// <inheritdoc />
        protected QueueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public QueueException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public QueueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}