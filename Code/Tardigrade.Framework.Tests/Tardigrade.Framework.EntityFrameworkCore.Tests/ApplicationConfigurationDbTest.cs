using Microsoft.EntityFrameworkCore;
using System;
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
        [InlineData("Test.NullValue")]
        public void GetBooleanSetting_NullValue_FormatException(string settingName)
        {
            Assert.Null(Config.GetAsBoolean(settingName));
        }

        [Theory]
        [InlineData("Test.NullValue")]
        public void GetEnumSetting_NullValue_ArgumentException(string settingName)
        {
            Assert.Null(Config.GetAsEnum<DayOfWeek>(settingName));
        }

        [Theory]
        [InlineData("Test.NullValue")]
        public void GetIntSetting_NullValue_FormatException(string settingName)
        {
            Assert.Null(Config.GetAsInt(settingName));
        }

        [Theory]
        [InlineData("database", "Test.Source")]
        public void GetSourceSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsString(settingName));
        }

        [Theory]
        [InlineData("Test.NullValue")]
        public void GetStringSetting_NullValue_Success(string settingName)
        {
            Assert.Null(Config.GetAsString(settingName));
        }
    }
}