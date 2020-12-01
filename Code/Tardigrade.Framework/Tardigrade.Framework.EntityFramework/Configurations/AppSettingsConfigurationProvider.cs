using Microsoft.Extensions.Configuration;
using System.Linq;
using Tardigrade.Framework.EntityFramework.Data;

namespace Tardigrade.Framework.EntityFramework.Configurations
{
    /// <summary>
    /// ConfigurationProvider that reads application settings from a database.
    /// <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#custom-configuration-provider">Custom configuration provider</a>
    /// </summary>
    public class AppSettingsConfigurationProvider : ConfigurationProvider
    {
        private string NameOrConnectionString { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="nameOrConnectionString">Name of the database connection or database connection string.</param>
        public AppSettingsConfigurationProvider(string nameOrConnectionString)
        {
            NameOrConnectionString = nameOrConnectionString;
        }

        /// <inheritdoc/>
        public override void Load()
        {
            using (var dbContext = new AppSettingsDbContext(NameOrConnectionString))
            {
                Data = dbContext.AppSettings.ToDictionary(c => c.Id, c => c.Value);
            }
        }
    }
}