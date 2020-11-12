using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Tardigrade.Framework.Configurations
{
    /// <summary>
    /// ConfigurationProvider based upon ConfigurationManager.
    /// <a href="https://benfoster.io/blog/net-core-configuration-legacy-projects/">Using .NET Core Configuration with legacy projects</a>
    /// <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#custom-configuration-provider">Custom configuration provider</a>
    /// </summary>
    public class LegacyConfigurationProvider : ConfigurationProvider
    {
        /// <inheritdoc/>
        public override void Load()
        {
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                Data.Add($"ConnectionStrings:{connectionString.Name}", connectionString.ConnectionString);
            }

            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                Data.Add(key, ConfigurationManager.AppSettings[key]);
            }
        }
    }
}