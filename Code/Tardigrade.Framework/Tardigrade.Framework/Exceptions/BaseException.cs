using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This class represents the base class for customised exceptions.
    /// <a href="https://docs.microsoft.com/en-au/archive/blogs/agileer/the-correct-way-to-code-a-custom-exception-class">The CORRECT Way to Code a Custom Exception Class</a>
    /// <a href="https://stackoverflow.com/questions/94488/what-is-the-correct-way-to-make-a-custom-net-exception-serializable">What is the correct way to make a custom .NET Exception serializable?</a>
    /// </summary>
    [Serializable]
    public abstract class BaseException : Exception
    {
        /// <summary>
        /// Unique reference for the exception.
        /// </summary>
        public string ExceptionReference { get; set; }

        /// <summary>
        /// <see cref="Exception()"/>
        /// </summary>
        protected BaseException()
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="Exception(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExceptionReference = info.GetString(nameof(ExceptionReference));
        }

        /// <summary>
        /// <see cref="Exception(string)"/>
        /// </summary>
        protected BaseException(string message) : base(message)
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="Exception(string, Exception)"/>
        /// </summary>
        protected BaseException(string message, Exception innerException) : base(message, innerException)
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// This method will generate a unique reference to be associated with an exception.
        /// </summary>
        /// <returns>A unique reference.</returns>
        private string GenerateUniqueReference()
        {
            return Math.Abs(Guid.NewGuid().GetHashCode()).ToString();
        }

        /// <summary>
        /// <see cref="Exception.GetObjectData(SerializationInfo, StreamingContext)"/>
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(ExceptionReference), ExceptionReference);

            base.GetObjectData(info, context);
        }
    }
}