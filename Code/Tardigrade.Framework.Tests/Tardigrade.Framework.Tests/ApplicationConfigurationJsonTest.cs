using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.Extensions;
using Tardigrade.Shared.Tests;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ApplicationConfigurationJsonTest : ApplicationConfigurationTest
    {
        public ApplicationConfigurationJsonTest() : base(new ApplicationConfiguration())
        {
        }

        [Theory]
        [InlineData("appsettings.json", "Test.Source")]
        public void GetSourceSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsString(settingName));
        }
    }
}