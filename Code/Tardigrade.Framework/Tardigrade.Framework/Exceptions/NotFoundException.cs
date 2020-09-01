using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with not found errors.
    /// </summary>
    [Serializable]
    public class NotFoundException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException()"/>
        /// </summary>
        protected NotFoundException()
        {
        }

        /// <summary>
        /// <see cref="BaseException(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public NotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}