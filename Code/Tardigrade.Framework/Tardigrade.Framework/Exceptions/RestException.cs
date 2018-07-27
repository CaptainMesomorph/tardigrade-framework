using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with REST API errors.
    /// </summary>
    [Serializable]
    public class RestException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public RestException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public RestException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}