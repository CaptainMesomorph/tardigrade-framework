using Microsoft.Extensions.Configuration;
using System.IO;

namespace Tardigrade.Framework.AzureStorage.Tests
{
    /// <summary>
    /// Configuration settings used by this application.
    /// </summary>
    internal class ApplicationConfiguration
    {
        private readonly IConfigurationRoot config;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="jsonFile">Name of the JSON configuration file.</param>
        /// <param name="basePath">Base path to the application configuration file. If null, the current directory is used.</param>
        public ApplicationConfiguration(string jsonFile = "appsettings.json", string basePath = null)
        {
            config = new ConfigurationBuilder()
                .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFile, true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        /// <summary>
        /// Azure Storage connection string.
        /// </summary>
        public string AzureStorageConnectionString => config["AzureWebJobsStorage"];

        /// <summary>
        /// Return the setting value if it exists. Return the default value if the setting key is null or empty, or the
        /// setting value does not exist.
        /// </summary>
        /// <param name="key">The setting key.</param>
        /// <param name="defaultValue">Default value.</param>
        public string GetAsString(string key, string defaultValue = null)
        {
            string value = defaultValue;

            if (!string.IsNullOrWhiteSpace(key))
            {
                value = config[key] ?? defaultValue;
            }

            return value;
        }
    }
}