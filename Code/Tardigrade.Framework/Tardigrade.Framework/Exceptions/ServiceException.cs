using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with service errors.
    /// </summary>
    [Serializable]
    public class ServiceException : BaseException
    {
        /// <inheritdoc />
        protected ServiceException()
        {
        }

        /// <inheritdoc />
        protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public ServiceException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}