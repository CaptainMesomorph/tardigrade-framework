using Microsoft.Extensions.Configuration;
using System;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Xunit;

namespace Tardigrade.Shared.Tests
{
    public abstract class ApplicationConfigurationTest
    {
        protected IConfiguration Config { get; }

        protected ApplicationConfigurationTest(IConfiguration config)
        {
            Config = config;
        }

        [Theory]
        [InlineData("Data Source=fake.database.windows.net;Initial Catalog=fake;User ID=fake;Password=fake;Encrypt=true;Trusted_Connection=false;", "DefaultConnection")]
        public void ConnectionString_Exists_Success(string expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetConnectionString(settingName));
        }

        [Theory]
        [InlineData("Test.Boolean.Index.False")]
        [InlineData("Test.EmptyValue")]
        [InlineData("Test.Source")]
        public void GetBooleanSetting_InvalidSettingValue_FormatException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsBoolean(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist")]
        public void GetBooleanSetting_SettingNameDoesNotExist_Success(string settingName)
        {
            Assert.Null(Config.GetAsBoolean(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist", false)]
        [InlineData("Test.Setting.Does.Not.Exist", true)]
        [InlineData("Test.Setting.Does.Not.Exist", null)]
        public void GetBooleanSetting_UseDefaultValue_Success(string settingName, bool? defaultValue)
        {
            Assert.Equal(defaultValue, Config.GetAsBoolean(settingName, defaultValue));
        }

        [Theory]
        [InlineData(false, "Test.Boolean.False")]
        [InlineData(true, "Test.Boolean.True")]
        public void GetBooleanSetting_ValidSetting_Success(bool expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsBoolean(settingName));
        }

        [Theory]
        [InlineData("Test.EmptyValue")]
        [InlineData("Test.Enum.IndexOutOfRange")]
        [InlineData("Test.Source")]
        public void GetEnumSetting_InvalidSettingValue_ArgumentException(string settingName)
        {
            Assert.Throws<ArgumentException>(() => Config.GetAsEnum<DayOfWeek>(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist")]
        public void GetEnumSetting_SettingNameDoesNotExist_Success(string settingName)
        {
            Assert.Null(Config.GetAsEnum<DayOfWeek>(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist", DayOfWeek.Tuesday)]
        [InlineData("Test.Setting.Does.Not.Exist", DayOfWeek.Wednesday)]
        [InlineData("Test.Setting.Does.Not.Exist", null)]
        public void GetEnumSetting_UseDefaultValue_Success(string settingName, DayOfWeek? defaultValue)
        {
            Assert.Equal(defaultValue, Config.GetAsEnum(settingName, defaultValue));
        }

        [Theory]
        [InlineData(DayOfWeek.Friday, "Test.Enum.Friday")]
        [InlineData(DayOfWeek.Saturday, "Test.Enum.Saturday")]
        [InlineData(DayOfWeek.Sunday, "Test.Enum.Sunday")]
        public void GetEnumSetting_ValidSetting_Success(DayOfWeek expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsEnum<DayOfWeek>(settingName));
        }

        [Theory]
        [InlineData("Test.EmptyValue")]
        [InlineData("Test.Source")]
        public void GetIntSetting_InvalidSettingValue_FormatException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsInt(settingName));
        }

        [Theory]
        [InlineData("Test.Int.Overflow")]
        public void GetIntSetting_InvalidSettingValue_OverflowException(string settingName)
        {
            Assert.Throws<OverflowException>(() => Config.GetAsInt(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist")]
        public void GetIntSetting_SettingNameDoesNotExist_Success(string settingName)
        {
            Assert.Null(Config.GetAsInt(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist", 42)]
        [InlineData("Test.Setting.Does.Not.Exist", 666)]
        [InlineData("Test.Setting.Does.Not.Exist", null)]
        public void GetIntSetting_UseDefaultValue_Success(string settingName, int? defaultValue)
        {
            Assert.Equal(defaultValue, Config.GetAsInt(settingName, defaultValue));
        }

        [Theory]
        [InlineData(2147483647, "Test.Int.Max")]
        [InlineData(-2147483648, "Test.Int.Min")]
        [InlineData(0, "Test.Int.Zero")]
        public void GetIntSetting_ValidSetting_Success(int expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsInt(settingName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetStringSetting_NullOrEmptySettingName_ArgumentNullException(string settingName)
        {
            Assert.Throws<ArgumentNullException>(() => Config.GetAsString(settingName));
        }

        [Theory]
        [InlineData("Test.Url.Reference.DoesNotExist")]
        [InlineData("Test.Url.Reference.DoesNotExistAsDot")]
        public void GetStringSetting_ReferencedSettingDoesNotExist_NotFoundException(string settingName)
        {
            Assert.Throws<NotFoundException>(() => Config.GetAsString(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist")]
        public void GetStringSetting_SettingNameDoesNotExist_Success(string settingName)
        {
            Assert.Null(Config.GetAsString(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist", "Dummy Default")]
        public void GetStringSetting_UseDefaultValue_Success(string settingName, string defaultValue)
        {
            Assert.Equal(defaultValue, Config.GetAsString(settingName, defaultValue));
        }

        [Theory]
        [InlineData("", "Test.EmptyValue")]
        [InlineData("https://test.kissmy.com", "Test.Url.Base.Api")]
        [InlineData("temporary", "Test.Url.Postfix.Temp")]
        [InlineData("https://test.kissmy.com/downloads", "Test.Url.Downloads")]
        [InlineData("https://test.kissmy.com/downloads/temporary", "Test.Url.Downloads.Temp")]
        [InlineData("https://test.kissmy.com/downloads/temporary", "Test.Url.Downloads.TempEmbed")]
        [InlineData("https://test.kissmy.com/downloads/temporary/temporary", "Test.Url.Downloads.TempTempEmbed")]
        [InlineData("/downloads", "Test.Url.Reference.EmptyValue")]
        [InlineData("${Invalid@Character}/downloads", "Test.Url.Reference.InvalidCharacters")]
        [InlineData("${}/downloads", "Test.Url.Reference.NoCharacters")]
        public void GetStringSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, Config.GetAsString(settingName));
        }
    }
}