using Microsoft.Extensions.Configuration;

namespace Tardigrade.Framework.EntityFramework.Configurations
{
    /// <summary>
    /// ConfigurationSource for a provider that reads application settings from a database.
    /// <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#custom-configuration-provider">Custom configuration provider</a>
    /// </summary>
    public class AppSettingsConfigurationSource : IConfigurationSource
    {
        private readonly string nameOrConnectionString;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name of the database connection or database connection string.</param>
        public AppSettingsConfigurationSource(string nameOrConnectionString)
        {
            this.nameOrConnectionString = nameOrConnectionString;
        }

        /// <inheritdoc/>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AppSettingsConfigurationProvider(nameOrConnectionString);
        }
    }
}