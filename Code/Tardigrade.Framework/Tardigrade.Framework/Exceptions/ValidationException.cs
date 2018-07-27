using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with model validation errors.
    /// </summary>
    [Serializable]
    public class ValidationException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public ValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}