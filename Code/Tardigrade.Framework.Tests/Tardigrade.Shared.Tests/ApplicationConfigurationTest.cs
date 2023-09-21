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
        protected IConfiguration NullConfig { get; }

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
        public void GetBooleanSetting_InvalidSettingValue_FormatException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsBoolean(settingName));
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
        [InlineData("Test.Enum.IndexOutOfRange")]
        public void GetEnumSetting_InvalidSettingValue_ArgumentException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsEnum<DayOfWeek>(settingName));
        }

        [Fact]
        public void GetEnumSetting_NotEnumeration_ArgumentException()
        {
            Assert.Throws<InvalidOperationException>(() => Config.GetAsEnum<int>("Test.Enum.Friday"));
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
        [InlineData("Test.Guid.Invalid")]
        public void GetGuidSetting_InvalidSettingValue_FormatException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsGuid(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist", "3c309004-a641-4b71-a4b9-c949f0c5a0a5")]
        public void GetGuidSetting_UseDefaultValue_Success(string settingName, string defaultValue)
        {
            Guid defaultGuid = Guid.Parse(defaultValue);
            Assert.Equal(defaultGuid, Config.GetAsGuid(settingName, defaultGuid));
        }

        [Theory]
        [InlineData("00000000000000000000000000000000", "Test.Guid.FormatN")]
        [InlineData("00000000-0000-0000-0000-000000000000", "Test.Guid.FormatD")]
        [InlineData("{00000000-0000-0000-0000-000000000000}", "Test.Guid.FormatB")]
        [InlineData("(00000000-0000-0000-0000-000000000000)", "Test.Guid.FormatP")]
        [InlineData("{0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}", "Test.Guid.FormatX")]
        [InlineData("d4d6030f-8a3b-4d12-b6e0-adf36aadee47", "Test.Guid.Random")]
        public void GetGuidSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            Guid expectedGuid = Guid.Parse(expectedValue);
            Assert.Equal(expectedGuid, Config.GetAsGuid(settingName));
        }

        [Theory]
        [InlineData("Test.Int.Overflow")]
        public void GetIntSetting_InvalidSettingValue_OverflowException(string settingName)
        {
            Assert.Throws<OverflowException>(() => Config.GetAsInt(settingName));
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
        [InlineData("Test.EmptyValue")]
        [InlineData("Test.Source")]
        public void GetSetting_InvalidSettingValue_FormatException(string settingName)
        {
            Assert.Throws<FormatException>(() => Config.GetAsBoolean(settingName));
            Assert.Throws<FormatException>(() => Config.GetAsEnum<DayOfWeek>(settingName));
            Assert.Throws<UriFormatException>(() => Config.GetAsUri(settingName));
            Assert.Throws<FormatException>(() => Config.GetAsGuid(settingName));
            Assert.Throws<FormatException>(() => Config.GetAsInt(settingName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetSetting_NullOrEmptySettingName_ArgumentNullException(string settingName)
        {
            Assert.Throws<ArgumentNullException>(() => Config.GetAsBoolean(settingName));
            Assert.Throws<ArgumentNullException>(() => Config.GetAsEnum<DayOfWeek>(settingName));
            Assert.Throws<ArgumentNullException>(() => Config.GetAsGuid(settingName));
            Assert.Throws<ArgumentNullException>(() => Config.GetAsInt(settingName));
            Assert.Throws<ArgumentNullException>(() => Config.GetAsString(settingName));
            Assert.Throws<ArgumentNullException>(() => Config.GetAsUri(settingName));
            Assert.Throws<ArgumentNullException>(() => NullConfig.GetAsBoolean(settingName));
            Assert.Throws<ArgumentNullException>(() => NullConfig.GetAsEnum<DayOfWeek>(settingName));
            Assert.Throws<ArgumentNullException>(() => NullConfig.GetAsGuid(settingName));
            Assert.Throws<ArgumentNullException>(() => NullConfig.GetAsInt(settingName));
            Assert.Throws<ArgumentNullException>(() => NullConfig.GetAsString(settingName));
            Assert.Throws<ArgumentNullException>(() => NullConfig.GetAsUri(settingName));
        }

        [Theory]
        [InlineData("Test.Url.Reference.DoesNotExist")]
        [InlineData("Test.Url.Reference.DoesNotExistAsDot")]
        public void GetSetting_ReferencedSettingDoesNotExist_NotFoundException(string settingName)
        {
            Assert.Throws<NotFoundException>(() => Config.GetAsBoolean(settingName));
            Assert.Throws<NotFoundException>(() => Config.GetAsEnum<DayOfWeek>(settingName));
            Assert.Throws<NotFoundException>(() => Config.GetAsGuid(settingName));
            Assert.Throws<NotFoundException>(() => Config.GetAsInt(settingName));
            Assert.Throws<NotFoundException>(() => Config.GetAsString(settingName));
            Assert.Throws<NotFoundException>(() => Config.GetAsUri(settingName));
        }

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist")]
        public void GetSetting_SettingNameDoesNotExist_Success(string settingName)
        {
            Assert.Null(Config.GetAsBoolean(settingName));
            Assert.Null(Config.GetAsEnum<DayOfWeek>(settingName));
            Assert.Null(Config.GetAsGuid(settingName));
            Assert.Null(Config.GetAsInt(settingName));
            Assert.Null(Config.GetAsString(settingName));
            Assert.Null(Config.GetAsUri(settingName));
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

        [Theory]
        [InlineData("Test.Setting.Does.Not.Exist", "https://test.kissmy.com/posts")]
        public void GetUriSetting_UseDefaultValue_Success(string settingName, string defaultValue)
        {
            var defaultUri = new Uri(defaultValue);
            Assert.Equal(defaultUri, Config.GetAsUri(settingName, defaultUri));
        }

        [Theory]
        [InlineData("https://test.kissmy.com", "Test.Url.Base.Api")]
        [InlineData("https://test.kissmy.com/downloads", "Test.Url.Downloads")]
        [InlineData("https://test.kissmy.com/downloads/temporary", "Test.Url.Downloads.Temp")]
        [InlineData("https://test.kissmy.com/downloads/temporary", "Test.Url.Downloads.TempEmbed")]
        [InlineData("https://test.kissmy.com/downloads/temporary/temporary", "Test.Url.Downloads.TempTempEmbed")]
        public void GetUriSetting_ValidSetting_Success(string expectedValue, string settingName)
        {
            var expectedUri = new Uri(expectedValue);
            Assert.Equal(expectedUri, Config.GetAsUri(settingName));
        }
    }
}