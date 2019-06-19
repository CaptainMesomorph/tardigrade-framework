using System;

namespace Tardigrade.Framework.Models.Errors
{
    /// <summary>
    /// Error associated with an operation.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Message associated with the error.
        /// </summary>
        public string Message { get; protected set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="message">Message associated with the error.</param>
        /// <exception cref="ArgumentNullException">message is null or empty.</exception>
        public Error(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            Message = message.Trim();
        }
    }
}