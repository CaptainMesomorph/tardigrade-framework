using System;
using System.Runtime.Serialization;

namespace Tardigrade.Framework.Exceptions
{
    /// <summary>
    /// This class represents the base class for customised exceptions.
    /// </summary>
    [Serializable]
    public abstract class BaseException : Exception
    {
        public string ExceptionReference { get; set; }

        /// <summary>
        /// <see cref="Exception()"/>
        /// </summary>
        public BaseException() : base()
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="Exception(string)"/>
        /// </summary>
        public BaseException(string message) : base(message)
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="Exception(string, Exception)"/>
        /// </summary>
        public BaseException(string message, Exception innerException) : base(message, innerException)
        {
            ExceptionReference = GenerateUniqueReference();
        }

        /// <summary>
        /// <see cref="Exception(SerializationInfo, StreamingContext)"/>
        /// </summary>
        protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
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
        /// This method will prefix the exception message with a unique reference.
        /// </summary>
        public override string Message
        {
            get
            {
                // Disable the exception reference until an appropriate framework for displaying exception messages in
                // the UI is implemented.
                //return $"[EXCEPTION_REF={ExceptionReference}] {base.Message}";
                return base.Message;
            }
        }
    }
}