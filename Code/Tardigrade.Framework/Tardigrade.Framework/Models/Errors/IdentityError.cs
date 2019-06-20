using System;

namespace Tardigrade.Framework.Models.Errors
{
    /// <summary>
    /// Error associated with operations on an application user identity.
    /// </summary>
    public class IdentityError : Error
    {
        /// <summary>
        /// Code associated with the error.
        /// </summary>
        public string Code { get; protected set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="code">Code associated with the error.</param>
        /// <param name="message">Message associated with the error.</param>
        /// <exception cref="ArgumentNullException">code and/or message is null or empty.</exception>
        public IdentityError(string code, string message) : base(message)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }

            Code = code;
        }
    }
}