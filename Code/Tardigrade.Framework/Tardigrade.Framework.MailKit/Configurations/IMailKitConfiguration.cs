namespace Tardigrade.Framework.MailKit.Configurations
{
    /// <summary>
    /// Configuration application properties associated with the MailKit library.
    /// </summary>
    public interface IMailKitConfiguration
    {
        /// <summary>
        /// SMTP mail server host name.
        /// </summary>
        string SmtpHost { get; set; }

        /// <summary>
        /// SMTP mail server password required for authentication.
        /// </summary>
        string SmtpPassword { get; set; }

        /// <summary>
        /// SMTP mail server port number.
        /// </summary>
        int SmtpPort { get; set; }

        /// <summary>
        /// SMTP mail server username required for authentication.
        /// </summary>
        string SmtpUsername { get; set; }
    }
}