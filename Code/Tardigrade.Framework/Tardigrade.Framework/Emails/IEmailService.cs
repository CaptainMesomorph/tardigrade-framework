using System.Threading.Tasks;

namespace Tardigrade.Framework.Emails
{
    /// <summary>
    /// Service interface for operations associated with email notifications.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send an email message.
        /// </summary>
        /// <param name="fromSender">From email address.</param>
        /// <param name="toRecipients">To email addresses.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="body">Email content as HTML.</param>
        /// <param name="ccRecipients">CC email addresses.</param>
        /// <param name="bccRecipients">BCC email addresses.</param>
        /// <param name="attachments">Email attachments.</param>
        /// <exception cref="System.ArgumentException">No "To" email addresses specified.</exception>
        /// <exception cref="System.ArgumentNullException">fromSender, toRecipients, subject and/or body are null.</exception>
        /// <exception cref="Exceptions.EmailFailedException">Error attempting to send email message.</exception>
        Task SendMessageAsync(
            EmailAddress fromSender,
            EmailAddress[] toRecipients,
            string subject,
            string body,
            EmailAddress[] ccRecipients = null,
            EmailAddress[] bccRecipients = null,
            EmailAttachment[] attachments = null);
    }
}