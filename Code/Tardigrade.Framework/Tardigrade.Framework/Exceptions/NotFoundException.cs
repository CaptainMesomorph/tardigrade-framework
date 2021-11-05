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
        /// <inheritdoc />
        protected NotFoundException()
        {
        }

        /// <inheritdoc />
        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public NotFoundException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}