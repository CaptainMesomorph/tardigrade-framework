using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with model validation errors.
    /// </summary>
    [Serializable]
    public class ValidationException : BaseException
    {
        /// <inheritdoc />
        protected ValidationException() : base()
        {
        }

        /// <inheritdoc />
        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public ValidationException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}