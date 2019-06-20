using System;

namespace Tardigrade.Framework.Models.Errors
{
    /// <summary>
    /// Error associated with an operation on an object.
    /// </summary>
    public class ObjectError<T, PK> : Error
    {
        /// <summary>
        /// Unique identifier of the object associated with the failed operation.
        /// </summary>
        public PK ObjectId { get; protected set; }

        /// <summary>
        /// Object associated with the failed operation.
        /// </summary>
        public T ObjectInstance { get; protected set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="objectId">Unique identifier of the object associated with the failed operation.</param>
        /// <param name="message">Message associated with the error.</param>
        /// <param name="objectInstance">Object associated with the failed operation.</param>
        /// <exception cref="ArgumentNullException">objectId is null, or message is null or empty.</exception>
        public ObjectError(PK objectId, string message, T objectInstance = default(T)) : base(message)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            ObjectId = objectId;
            ObjectInstance = objectInstance;
        }
    }
}