using System.IO;

namespace Tardigrade.Framework.Emails
{
    /// <summary>
    /// This class represents an email attachment.
    /// </summary>
    public class EmailAttachment
    {
        /// <summary>
        /// Content of the email attachment.
        /// </summary>
        public Stream Content { get; set; }

        /// <summary>
        /// File name associated with the attachment.
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Media type that indicates the format of the attachment.
        /// </summary>
        public string MimeType { get; set; }
    }
}