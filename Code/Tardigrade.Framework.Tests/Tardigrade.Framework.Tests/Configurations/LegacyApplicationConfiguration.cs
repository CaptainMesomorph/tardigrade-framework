using System.Reflection;
using Tardigrade.Framework.Configurations;

namespace Tardigrade.Framework.Tests.Configurations
{
    public class LegacyApplicationConfiguration : ApplicationConfiguration
    {
        protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

        public LegacyApplicationConfiguration() : base(new LegacySettingsConfigurationSource())
        {
        }
    }
}