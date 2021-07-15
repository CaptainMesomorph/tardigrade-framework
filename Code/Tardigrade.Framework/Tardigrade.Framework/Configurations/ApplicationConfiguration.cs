using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Linq;

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
        /// Load application settings into this class from the provided configuration sources in the order specified.
        /// If a configuration source is loaded and the key already exists from a previous source, the latest value
        /// overwrites the previous value. If no configuration sources are provided, then the following default sources
        /// will be used.
        ///
        ///   1. app.config file
        ///   2. appsettings.json file
        ///   3. appsettings.{env.EnvironmentName}.json file
        ///   4. The local User Secrets File (only in local development environment)
        ///   5. Environment Variables
        ///   6. Command Line Arguments
        ///
        /// If configuration sources are provided, then they will replace the app.config file source. The remaining
        /// configuration sources remain.
        ///
        /// <a href="https://devblogs.microsoft.com/premier-developer/order-of-precedence-when-configuring-asp-net-core/">Order of Precedence when Configuring ASP.NET Core</a>
        /// </summary>
        /// <param name="configurationSources">Collection of configuration key/value sources.</param>
        public ApplicationConfiguration(params IConfigurationSource[] configurationSources)
        {
            if (configurationSources.Any())
            {
                var builder = new ConfigurationBuilder();

                foreach (IConfigurationSource configurationSource in configurationSources)
                {
                    builder.Add(configurationSource);
                }

                builder.AddJsonFile(DefaultJsonFile, true, true);
                builder.AddEnvironmentVariables();
                config = builder.Build();
            }
            else
            {
                config = new ConfigurationBuilder()
                    .Add(new LegacySettingsConfigurationSource())
                    .AddJsonFile(DefaultJsonFile, true, true)
                    .AddEnvironmentVariables()
                    .Build();
            }
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