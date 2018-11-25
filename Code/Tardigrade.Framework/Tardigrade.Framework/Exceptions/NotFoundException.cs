using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with not found errors.
    /// </summary>
    [Serializable]
    public class NotFoundException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public NotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}