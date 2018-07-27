using System;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with repository errors.
    /// </summary>
    [Serializable]
    public class RepositoryException : BaseException
    {
        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        public RepositoryException(string message) : base(message)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        public RepositoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}