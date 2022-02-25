using Microsoft.Extensions.Configuration;
using System.Reflection;
using Tardigrade.Framework.Configurations;

namespace Tardigrade.Framework.Testing
{
    /// <summary>
    /// Configuration settings specifically for unit testing in a development environment.
    /// </summary>
    public class UnitTestApplicationConfiguration : ApplicationConfiguration
    {
        /// <inheritdoc />
        protected UnitTestApplicationConfiguration(IConfiguration configuration) : base(configuration)
        {
        }

        /// <inheritdoc />
        public UnitTestApplicationConfiguration(
            Assembly userSecretsAssembly,
            params IConfigurationSource[] configurationSources)
            : base(userSecretsAssembly, configurationSources)
        {
        }
    }
}