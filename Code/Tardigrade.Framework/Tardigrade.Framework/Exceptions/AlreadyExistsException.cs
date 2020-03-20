using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with create operations on items which already exist.
    /// </summary>
    [Serializable]
    public class AlreadyExistsException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException()"/>
        /// </summary>
        protected AlreadyExistsException()
        {
        }

        /// <summary>
        /// <see cref="BaseException(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected AlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public AlreadyExistsException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public AlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}