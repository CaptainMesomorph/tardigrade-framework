using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tardigrade.Framework.Configurations
{
    /// <summary>
    /// Configuration settings used by an application.
    /// </summary>
    public class ApplicationConfiguration : IConfiguration
    {
        private IConfiguration _config;

        /// <inheritdoc/>
        public string this[string key]
        {
            get => _config[key];
            set => _config[key] = value;
        }

        /// <summary>
        /// The assembly associated with the User Secrets.
        /// When running unit tests from Visual Studio, use Assembly.GetExecutingAssembly() as
        /// Assembly.GetEntryAssembly() returns "testhost".
        /// </summary>
        protected Assembly UserSecretsAssembly { get; set; } = Assembly.GetEntryAssembly();

        /// <summary>
        /// Explicitly define a default constructor to prevent defaulting to the
        /// ApplicationConfiguration(params IConfigurationSource[]) constructor when defining a new constructor in a
        /// derived class.
        /// </summary>
        protected ApplicationConfiguration()
        {
        }

        /// <summary>
        /// Create an instance of this class based upon an existing configuration.
        /// </summary>
        /// <param name="configuration">Existing application configuration properties.</param>
        /// <exception cref="ArgumentNullException">configuration is null.</exception>
        protected ApplicationConfiguration(IConfiguration configuration)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Create an instance of this class by loading application settings in the order specified below.
        ///
        ///   1. appsettings.json file
        ///   2. appsettings.{environment}.json file
        ///   3. Custom configuration sources (in the order defined)
        ///   4. The local User Secrets File (only in local development environment)
        ///   5. Environment Variables
        ///
        /// If a configuration source is loaded and the key already exists from a previous source, the latest value
        /// overwrites the previous value.
        /// </summary>
        /// <param name="userSecretsAssembly">The assembly associated with the User Secrets.</param>
        /// <param name="configurationSources">Collection of custom configuration key/value sources.</param>
        protected ApplicationConfiguration(
            Assembly userSecretsAssembly = null,
            params IConfigurationSource[] configurationSources)
        {
            if (userSecretsAssembly != null)
            {
                // If a User Secret assembly is specified, assume a development environment. There is a known issue
                // whereby the Test Runner does not reference "launchSettings.json" to check for the current
                // environment (https://github.com/aspnet/Tooling/issues/1089).
                Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
                UserSecretsAssembly = userSecretsAssembly;
            }

            // Create a host from which to load up configuration and register services.
            IHost host = CreateHostBuilder(configurationSources).Build();
            Task.Run(() => host.RunAsync());
        }

        /// <summary>
        /// Create an instance of this class by loading application settings in the order specified below.
        ///
        ///   1. appsettings.json file
        ///   2. appsettings.{environment}.json file
        ///   3. Custom configuration sources (in the order defined)
        ///   4. Environment Variables
        ///
        /// If a configuration source is loaded and the key already exists from a previous source, the latest value
        /// overwrites the previous value.
        /// </summary>
        /// <param name="configurationSources">Collection of custom configuration key/value sources.</param>
        public ApplicationConfiguration(params IConfigurationSource[] configurationSources)
            : this(null, configurationSources)
        {
        }

        /// <summary>
        /// Create a host builder that correctly loads User Secrets for unit testing.
        /// </summary>
        /// <returns>Host builder.</returns>
        protected IHostBuilder CreateHostBuilder(params IConfigurationSource[] configurationSources) => Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                if (configurationSources.Any())
                {
                    foreach (IConfigurationSource configurationSource in configurationSources)
                    {
                        config.Add(configurationSource);
                    }
                }

                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    config.AddUserSecrets(UserSecretsAssembly, true, true);
                }

                config.AddEnvironmentVariables();

                _config = config.Build();
            });

        /// <inheritdoc/>
        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _config.GetChildren();
        }

        /// <inheritdoc/>
        public IChangeToken GetReloadToken()
        {
            return _config.GetReloadToken();
        }

        /// <inheritdoc/>
        public IConfigurationSection GetSection(string key)
        {
            return _config.GetSection(key);
        }
    }
}