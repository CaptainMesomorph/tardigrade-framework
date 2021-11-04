using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with duplicate items.
    /// </summary>
    [Serializable]
    public class DuplicateFoundException : BaseException
    {
        /// <inheritdoc />
        protected DuplicateFoundException()
        {
        }

        /// <inheritdoc />
        protected DuplicateFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public DuplicateFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public DuplicateFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}