using System;
using System.Reflection;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Testing;
using Tardigrade.Shared.Tests;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ApplicationConfigurationLegacyTest : ApplicationConfigurationTest
    {
        public ApplicationConfigurationLegacyTest()
            : base(new UnitTestApplicationConfiguration(
                Assembly.GetExecutingAssembly(),
                new LegacySettingsConfigurationSource()))
        {
        }

        [Theory]
        [InlineData("Test.NullValue")]
        public void GetBooleanSetting_NullValue_FormatException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsBoolean(settingName));
        }

        [Theory]
        [InlineData("Test.NullValue")]
        public void GetEnumSetting_NullValue_ArgumentException(string settingName)
        {
            Assert.Throws<ArgumentException>(() => Config.GetAsEnum<DayOfWeek>(settingName));
        }

        [Theory]
        [InlineData("Test.NullValue")]
        public void GetIntSetting_NullValue_FormatException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsInt(settingName));
        }

        [Theory]
        [InlineData("app.config", "Test.Source")]
        public void GetSourceSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsString(settingName));
        }

        [Theory]
        [InlineData("Test.NullValue")]
        public void GetStringSetting_NullValue_Success(string settingName)
        {
            Assert.Equal(string.Empty, Config.GetAsString(settingName));
        }
    }
}