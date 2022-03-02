using Microsoft.Extensions.Configuration;
using System.Configuration;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.Extensions;

namespace Tardigrade.Framework.AzureStorage.Configurations
{
    /// <summary>
    /// Configuration settings associated with Azure Storage accounts.
    /// </summary>
    public class AzureStorageConfiguration : ApplicationConfiguration, IAzureStorageConfiguration
    {
        private const string AzureStorageConnectionStringKey = "AzureStorage.ConnectionString";

        /// <inheritdoc />
        public string AzureStorageConnectionString
        {
            get => this.GetAsString(AzureStorageConnectionStringKey) ??
                   throw new ConfigurationErrorsException(
                       $"Missing application setting: {AzureStorageConnectionStringKey}.");
            set => this[AzureStorageConnectionStringKey] = value;
        }

        /// <inheritdoc />
        public AzureStorageConfiguration(IConfiguration configuration) : base(configuration)
        {
        }
    }
}