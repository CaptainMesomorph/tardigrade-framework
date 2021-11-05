using Microsoft.Extensions.Configuration;
using System.Configuration;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.Extensions;

namespace Tardigrade.Framework.MailKit.Configurations
{
    /// <inheritdoc cref="IMailKitConfiguration" />
    public class MailKitConfiguration : ApplicationConfiguration, IMailKitConfiguration
    {
        private const string HostKey = "MailKit.Smtp.Host";
        private const string PasswordKey = "MailKit.Smtp.Password";
        private const string PortKey = "MailKit.Smtp.Port";
        private const string UsernameKey = "MailKit.Smtp.SmtpUsername";

        /// <inheritdoc />
        public MailKitConfiguration(IConfiguration configuration) : base(configuration)
        {
        }

        /// <inheritdoc />
        public string SmtpHost
        {
            get => this.GetAsString(HostKey) ??
                   throw new ConfigurationErrorsException($"Missing application setting: {HostKey}.");
            set => this[HostKey] = value;
        }

        /// <inheritdoc />
        public string SmtpPassword
        {
            get => this.GetAsString(PasswordKey) ??
                   throw new ConfigurationErrorsException($"Missing application setting: {PasswordKey}.");
            set => this[PasswordKey] = value;
        }

        /// <inheritdoc />
        public int SmtpPort
        {
            get => this.GetAsInt(PortKey) ??
                   throw new ConfigurationErrorsException($"Missing application setting: {PortKey}.");
            set => this[PortKey] = value.ToString();
        }

        /// <inheritdoc />
        public string SmtpUsername
        {
            get => this.GetAsString(UsernameKey) ??
                   throw new ConfigurationErrorsException($"Missing application setting: {UsernameKey}.");
            set => this[UsernameKey] = value;
        }
    }
}