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
        /// <summary>
        /// <see cref="BaseException()"/>
        /// </summary>
        protected ServiceException()
        {
        }

        /// <summary>
        /// <see cref="BaseException(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected ServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public ServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}