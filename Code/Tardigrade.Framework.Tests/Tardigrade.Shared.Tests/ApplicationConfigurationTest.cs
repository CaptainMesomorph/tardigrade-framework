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
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetStringSetting_NullOrEmptySettingName_ArgumentNullException(string settingName)
        {
            Assert.Throws<ArgumentNullException>(() => Config.GetAsString(settingName));
        }

        /// <summary>
        /// For some reason, null values read from appsettings.json and app.config are treated as empty ("") strings.
        /// However, null columns read from a database are treated as null.
        /// </summary>
        /// <param name="settingName">Setting name.</param>
        [Theory]
        [InlineData("Test.NoValue")]
        public void GetStringSetting_NullSettingValue_Success(string settingName)
        {
            Assert.True(string.IsNullOrEmpty(Config.GetAsString(settingName)));
        }

        [Theory]
        [InlineData("Test.Url.Reference.DoesNotExist")]
        [InlineData("Test.Url.Reference.DoesNotExistAsDot")]
        public void GetStringSetting_ReferencedSettingDoesNotExist_NotFoundException(string settingName)
        {
            Assert.Throws<NotFoundException>(() => Config.GetAsString(settingName));
        }

        [Theory]
        [InlineData("Test.Url.Does.Not.Exist")]
        public void GetStringSetting_SettingNameDoesNotExist_Success(string settingName)
        {
            Assert.Null(Config.GetAsString(settingName));
        }

        [Theory]
        [InlineData("Dummy Default", "Test.Url.Does.Not.Exist", "Dummy Default")]
        public void GetStringSetting_UseDefaultValue_Success(
            string expectedValue,
            string settingName,
            string defaultValue)
        {
            Assert.Equal(expectedValue, Config.GetAsString(settingName, defaultValue));
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