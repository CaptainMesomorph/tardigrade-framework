using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception represents a sign in attempt that needs two-factor authentication.
    /// </summary>
    [Serializable]
    public class TwoFactorRequiredException : BaseException
    {
        /// <inheritdoc />
        protected TwoFactorRequiredException()
        {
        }

        /// <inheritdoc />
        protected TwoFactorRequiredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public TwoFactorRequiredException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public TwoFactorRequiredException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}