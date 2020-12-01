using Microsoft.EntityFrameworkCore;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.EntityFrameworkCore.Configurations;
using Tardigrade.Framework.Extensions;
using Tardigrade.Shared.Tests;
using Xunit;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests
{
    public class ApplicationConfigurationDbTest : ApplicationConfigurationTest
    {
        public ApplicationConfigurationDbTest() : base(
            new ApplicationConfiguration(
                new AppSettingsConfigurationSource(
                    options => options.UseSqlite("Data Source=EntityFrameworkCoreTest.db"))))
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