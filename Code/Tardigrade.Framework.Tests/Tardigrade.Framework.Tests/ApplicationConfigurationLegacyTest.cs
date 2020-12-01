using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.Extensions;
using Tardigrade.Shared.Tests;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ApplicationConfigurationLegacyTest : ApplicationConfigurationTest
    {
        public ApplicationConfigurationLegacyTest()
            : base(new ApplicationConfiguration(new LegacySettingsConfigurationSource()))
        {
        }

        [Theory]
        [InlineData("app.config", "Test.Source")]
        public void GetSourceSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsString(settingName));
        }
    }
}