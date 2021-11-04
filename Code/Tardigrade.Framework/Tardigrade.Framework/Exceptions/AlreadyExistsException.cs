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
        /// <inheritdoc />
        protected AlreadyExistsException()
        {
        }

        /// <inheritdoc />
        protected AlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public AlreadyExistsException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public AlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}