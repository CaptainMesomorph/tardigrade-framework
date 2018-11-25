using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with duplicate items.
    /// </summary>
    [Serializable]
    public class DuplicateFoundException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public DuplicateFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public DuplicateFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}