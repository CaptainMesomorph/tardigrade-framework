using Microsoft.Extensions.Configuration;

namespace Tardigrade.Framework.Configurations
{
    /// <summary>
    /// ConfigurationSource for a LegacyConfigurationProvider.
    /// <a href="https://benfoster.io/blog/net-core-configuration-legacy-projects/">Using .NET Core Configuration with legacy projects</a>
    /// <a href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#custom-configuration-provider">Custom configuration provider</a>
    /// </summary>
    public class LegacyConfigurationSource : IConfigurationSource
    {
        /// <inheritdoc/>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new LegacyConfigurationProvider();
        }
    }
}