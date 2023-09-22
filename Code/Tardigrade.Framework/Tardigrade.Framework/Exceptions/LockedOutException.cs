using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception represents a sign in attempt that failed because the user was locked out.
    /// </summary>
    [Serializable]
    public class LockedOutException : BaseException
    {
        /// <inheritdoc />
        protected LockedOutException() : base()
        {
        }

        /// <inheritdoc />
        protected LockedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public LockedOutException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public LockedOutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}