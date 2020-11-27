using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.EntityFramework.Configurations;
using Tardigrade.Framework.Extensions;
using Tardigrade.Shared.Tests;
using Xunit;

namespace Tardigrade.Framework.EntityFramework.Tests
{
    public class ApplicationConfigurationDbTest : ApplicationConfigurationTest
    {
        public ApplicationConfigurationDbTest()
            : base(new ApplicationConfiguration(new AppSettingsConfigurationSource("name=EntityFrameworkTest")))
        {
        }

        [Theory]
        [InlineData("database", "Test.Source")]
        public void GetSourceSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsString(settingName));
        }
    }
}