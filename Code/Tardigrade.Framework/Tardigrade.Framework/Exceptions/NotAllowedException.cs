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
        /// <summary>
        /// <see cref="BaseException()"/>
        /// </summary>
        protected NotAllowedException()
        {
        }

        /// <summary>
        /// <see cref="BaseException(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected NotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public NotAllowedException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public NotAllowedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}