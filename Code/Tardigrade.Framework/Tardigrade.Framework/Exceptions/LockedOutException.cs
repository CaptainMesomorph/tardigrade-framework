using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception represents a sign in attempt that failed because the user was locked out.
    /// </summary>
    [Serializable]
    public class LockedOutException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public LockedOutException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public LockedOutException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}