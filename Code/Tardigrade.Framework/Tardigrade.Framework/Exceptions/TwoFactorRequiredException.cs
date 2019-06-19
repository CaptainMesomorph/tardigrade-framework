using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception represents a sign in attempt that needs two-factor authentication.
    /// </summary>
    [Serializable]
    public class TwoFactorRequiredException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public TwoFactorRequiredException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public TwoFactorRequiredException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}