using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Tardigrade.Framework.EntityFrameworkCore.Configurations
{
    /// <summary>
    /// ConfigurationSource for a provider that reads application settings from a database.
    /// <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0#custom-configuration-provider">Custom configuration provider</a>
    /// </summary>
    public class AppSettingsConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="optionsAction">Options associated with the database context.</param>
        public AppSettingsConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            _optionsAction = optionsAction;
        }

        /// <inheritdoc/>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AppSettingsConfigurationProvider(_optionsAction);
        }
    }
}