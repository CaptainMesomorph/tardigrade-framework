using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.IO;

namespace Tardigrade.Framework.Configurations
{
    /// <summary>
    /// Configuration settings used by an application.
    /// </summary>
    public class ApplicationConfiguration : IConfiguration
    {
        private const string DefaultJsonFile = "appsettings.json";

        private readonly IConfigurationRoot config;

        /// <inheritdoc/>
        public string this[string key] { get => config[key]; set => config[key] = value; }

        /// <summary>
        /// Load application settings into this class from the following configuration sources in the order specified.
        /// If a configuration source is loaded and the key already exists from a previous source, the latest value
        /// overwrites the previous value.
        ///
        ///   1. app.config file
        ///   2. appsettings.json file
        ///   3. appsettings.{env.EnvironmentName}.json file
        ///   4. The local User Secrets File (only in local development environment)
        ///   5. Environment Variables
        ///   6. Command Line Arguments
        ///
        /// <a href="https://devblogs.microsoft.com/premier-developer/order-of-precedence-when-configuring-asp-net-core/">Order of Precedence when Configuring ASP.NET Core</a>
        /// </summary>
        /// <param name="jsonFile">Name of the JSON configuration file.</param>
        /// <param name="basePath">Base path to the application configuration file. If null, the current directory is used.</param>
        public ApplicationConfiguration(string jsonFile = DefaultJsonFile, string basePath = null)
        {
            config = new ConfigurationBuilder()
                .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
                .Add(new LegacyConfigurationSource())
                .AddJsonFile(jsonFile, true, true)
                .AddEnvironmentVariables()
                .Build();
        }

        /// <inheritdoc/>
        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return config.GetChildren();
        }

        /// <inheritdoc/>
        public IChangeToken GetReloadToken()
        {
            return config.GetReloadToken();
        }

        /// <inheritdoc/>
        public IConfigurationSection GetSection(string key)
        {
            return config.GetSection(key);
        }
    }
}