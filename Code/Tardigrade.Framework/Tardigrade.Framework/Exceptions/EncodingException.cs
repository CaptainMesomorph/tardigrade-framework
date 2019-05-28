using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with encoding errors.
    /// </summary>
    [Serializable]
    public class EncodingException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public EncodingException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public EncodingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}