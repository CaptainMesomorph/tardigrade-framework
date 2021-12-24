using System.ComponentModel.DataAnnotations;

namespace Tardigrade.Framework.Emails
{
    /// <summary>
    /// This class represents an email address.
    /// </summary>
    public class EmailAddress
    {
        /// <summary>
        /// Email address.
        /// </summary>
        [Required]
        public string Address { get; set; }

        /// <summary>
        /// Display name associated with the email address.
        /// </summary>
        public string Name { get; set; }
    }
}