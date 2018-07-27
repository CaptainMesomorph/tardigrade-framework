using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with service errors.
    /// </summary>
    [Serializable]
    public class ServiceException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public ServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public ServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}