using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception represents a sign in attempt that failed because the user is not allowed to sign-in. This may
    /// occur, for instance, where their email has yet to be confirmed.
    /// </summary>
    [Serializable]
    public class NotAllowedException : BaseException
    {
        /// <inheritdoc />
        protected NotAllowedException()
        {
        }

        /// <inheritdoc />
        protected NotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public NotAllowedException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public NotAllowedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}