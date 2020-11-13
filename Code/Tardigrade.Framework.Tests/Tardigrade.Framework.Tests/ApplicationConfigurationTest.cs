using Microsoft.Extensions.Configuration;
using System;
using Tardigrade.Framework.Configurations;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ApplicationConfigurationTest
    {
        /// <summary>
        /// Create an instance of the ApplicationConfiguration class.
        /// </summary>
        /// <param name="useAppConfigOnly">Flag indicating whether to force settings to be read from app.config only.</param>
        /// <returns>Instance of the ApplicationConfiguration class.</returns>
        private static ApplicationConfiguration CreateInstance(bool useAppConfigOnly)
        {
            return useAppConfigOnly ? new ApplicationConfiguration("fake.json") : new ApplicationConfiguration();
        }

        [Theory]
        [InlineData(true, "Data Source=fake.database.windows.net;Initial Catalog=fake;User ID=fake;Password=fake;Encrypt=true;Trusted_Connection=false;", "DefaultConnection")]
        [InlineData(false, "Data Source=fake.database.windows.net;Initial Catalog=fake;User ID=fake;Password=fake;Encrypt=true;Trusted_Connection=false;", "DefaultConnection")]
        public void ConnectionString_Exists_Success(bool useAppConfigOnly, string expectedValue, string settingName)
        {
            ApplicationConfiguration config = CreateInstance(useAppConfigOnly);
            Assert.Equal(expectedValue, config.GetConnectionString(settingName));
        }

        [Theory]
        [InlineData(true, null)]
        [InlineData(true, "")]
        [InlineData(true, "   ")]
        [InlineData(false, null)]
        [InlineData(false, "")]
        [InlineData(false, "   ")]
        public void GetStringSetting_NullOrEmptySettingName_ArgumentNullException(
            bool useAppConfigOnly,
            string settingName)
        {
            ApplicationConfiguration config = CreateInstance(useAppConfigOnly);
            Assert.Throws<ArgumentNullException>(() => config.GetAsString(settingName));
        }

        [Theory]
        [InlineData(true, "Test.Url.Reference.DoesNotExist")]
        [InlineData(true, "Test.Url.Reference.DoesNotExistAsDot")]
        [InlineData(false, "Test.Url.Reference.DoesNotExist")]
        [InlineData(false, "Test.Url.Reference.DoesNotExistAsDot")]
        public void GetStringSetting_ReferencedSettingDoesNotExist_NotFoundException(
            bool useAppConfigOnly,
            string settingName)
        {
            ApplicationConfiguration config = CreateInstance(useAppConfigOnly);
            Assert.Throws<NotFoundException>(() => config.GetAsString(settingName));
        }

        [Theory]
        [InlineData(true, "Test.Url.Does.Not.Exist")]
        [InlineData(false, "Test.Url.Does.Not.Exist")]
        public void GetStringSetting_SettingDoesNotExist_Success(bool useAppConfigOnly, string settingName)
        {
            ApplicationConfiguration config = CreateInstance(useAppConfigOnly);
            Assert.Null(config.GetAsString(settingName));
        }

        [Theory]
        [InlineData(true, "Dummy Default", "Test.Url.Does.Not.Exist", "Dummy Default")]
        [InlineData(false, "Dummy Default", "Test.Url.Does.Not.Exist", "Dummy Default")]
        public void GetStringSetting_UseDefaultValue_Success(
            bool useAppConfigOnly,
            string expectedValue,
            string settingName,
            string defaultValue)
        {
            ApplicationConfiguration config = CreateInstance(useAppConfigOnly);
            Assert.Equal(expectedValue, config.GetAsString(settingName, defaultValue));
        }

        [Theory]
        [InlineData(true, "https://test-config.kissmy.com", "Test.Url.Base.Api")]
        [InlineData(true, "temporary", "Test.Url.Postfix.Temp")]
        [InlineData(true, "https://test-config.kissmy.com/downloads", "Test.Url.Downloads")]
        [InlineData(true, "https://test-config.kissmy.com/downloads/temporary", "Test.Url.Downloads.Temp")]
        [InlineData(true, "https://test-config.kissmy.com/downloads/temporary", "Test.Url.Downloads.TempEmbed")]
        [InlineData(true, "https://test-config.kissmy.com/downloads/temporary/temporary", "Test.Url.Downloads.TempTempEmbed")]
        [InlineData(true, "", "Test.Url.NoValue")]
        [InlineData(true, "", "Test.Url.EmptyValue")]
        [InlineData(true, "/downloads", "Test.Url.Reference.EmptyValue")]
        [InlineData(true, "${Invalid@Character}/downloads", "Test.Url.Reference.InvalidCharacters")]
        [InlineData(true, "${}/downloads", "Test.Url.Reference.NoCharacters")]
        [InlineData(false, "https://test-json.kissmy.com", "Test.Url.Base.Api")]
        [InlineData(false, "temporary", "Test.Url.Postfix.Temp")]
        [InlineData(false, "https://test-json.kissmy.com/downloads", "Test.Url.Downloads")]
        [InlineData(false, "https://test-json.kissmy.com/downloads/temporary", "Test.Url.Downloads.Temp")]
        [InlineData(false, "https://test-json.kissmy.com/downloads/temporary", "Test.Url.Downloads.TempEmbed")]
        [InlineData(false, "https://test-json.kissmy.com/downloads/temporary/temporary", "Test.Url.Downloads.TempTempEmbed")]
        [InlineData(false, "", "Test.Url.NoValue")]
        [InlineData(false, "", "Test.Url.EmptyValue")]
        [InlineData(false, "/downloads", "Test.Url.Reference.EmptyValue")]
        [InlineData(false, "${Invalid@Character}/downloads", "Test.Url.Reference.InvalidCharacters")]
        [InlineData(false, "${}/downloads", "Test.Url.Reference.NoCharacters")]
        public void GetStringSetting_ValidSetting_Success(
            bool useAppConfigOnly,
            string expectedValue,
            string settingName)
        {
            ApplicationConfiguration config = CreateInstance(useAppConfigOnly);
            Assert.Equal(expectedValue, config.GetAsString(settingName));
        }
    }
}