using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Tardigrade.Framework.EntityFrameworkCore.Data;

namespace Tardigrade.Framework.EntityFrameworkCore.Configurations
{
    /// <summary>
    /// ConfigurationProvider that reads application settings from a database.
    /// <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#custom-configuration-provider">Custom configuration provider</a>
    /// </summary>
    public class AppSettingsConfigurationProvider : ConfigurationProvider
    {
        private Action<DbContextOptionsBuilder> OptionsAction { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="optionsAction">Options associated with the database context.</param>
        public AppSettingsConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        /// <inheritdoc/>
        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<AppSettingsDbContext>();
            OptionsAction(builder);
            using var dbContext = new AppSettingsDbContext(builder.Options);
            Data = dbContext.AppSettings.ToDictionary(a => a.Id, a => a.Value);
        }
    }
}