using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions.Security
{
    /// <summary>
    /// This exception is associated with authenticcation errors.
    /// </summary>
    public class AuthenticationException : SecurityException
    {
        /// <inheritdoc />
        protected AuthenticationException() : base()
        {
        }

        /// <inheritdoc />
        protected AuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public AuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <inheritdoc />
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}