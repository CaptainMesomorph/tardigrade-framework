using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Tardigrade.Framework.EntityFrameworkCore.Configurations
{
    /// <summary>
    /// ConfigurationSource for a provider that reads application settings from a database.
    /// <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#custom-configuration-provider">Custom configuration provider</a>
    /// </summary>
    public class AppSettingsConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> optionsAction;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="optionsAction">Options associated with the database context.</param>
        public AppSettingsConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            this.optionsAction = optionsAction;
        }

        /// <inheritdoc/>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AppSettingsConfigurationProvider(optionsAction);
        }
    }
}