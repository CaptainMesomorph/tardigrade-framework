using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with application security errors.
    /// </summary>
    [Serializable]
    public class SecurityException : BaseException
    {
        /// <inheritdoc />
        protected SecurityException()
        {
        }

        /// <inheritdoc />
        protected SecurityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public SecurityException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public SecurityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}