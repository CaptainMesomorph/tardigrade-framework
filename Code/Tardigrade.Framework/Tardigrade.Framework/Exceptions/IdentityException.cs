using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Tardigrade.Framework.Models.Errors;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This exception is associated with application user identity errors.
    /// </summary>
    [Serializable]
    public class IdentityException : BaseException
    {
        /// <summary>
        /// Collection of errors associated with the exception.
        /// </summary>
        public IEnumerable<IdentityError> Errors { get; protected set; }

        /// <inheritdoc />
        protected IdentityException() : base()
        {
        }

        /// <inheritdoc />
        protected IdentityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Errors = (IEnumerable<IdentityError>)info.GetValue(nameof(Errors), typeof(IEnumerable<IdentityError>));
        }

        /// <inheritdoc />
        protected IdentityException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        protected IdentityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// <see cref="BaseException(string, Exception)"/>
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        /// <param name="errors">Collection of errors associated with the exception.</param>
        public IdentityException(string message, Exception innerException, IEnumerable<IdentityError> errors = null)
            : base(message, innerException)
        {
            Errors = errors;
        }

        /// <summary>
        /// <see cref="BaseException(string)"/>
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errors">Collection of errors associated with the exception.</param>
        public IdentityException(string message, IEnumerable<IdentityError> errors = null) : base(message)
        {
            Errors = errors;
        }

        /// <summary>
        /// <see cref="Exception.GetObjectData(SerializationInfo, StreamingContext)"/>
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Errors), Errors, typeof(IEnumerable<IdentityError>));

            base.GetObjectData(info, context);
        }
    }
}