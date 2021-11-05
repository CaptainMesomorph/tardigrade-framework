using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with repository errors.
    /// </summary>
    [Serializable]
    public class RepositoryException : BaseException
    {
        /// <inheritdoc />
        protected RepositoryException()
        {
        }

        /// <inheritdoc />
        protected RepositoryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <inheritdoc />
        public RepositoryException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public RepositoryException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}