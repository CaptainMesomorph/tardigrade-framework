using System;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Tests.Models;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ExtensionTest
    {
        [Fact]
        public void ConvertEnum_DisplayAttribute_Success()
        {
            string locale = LocaleType.EnGb.ToName();
            var localeEnum = locale.ToEnum<LocaleType>();
            Assert.Equal(LocaleType.EnGb, localeEnum);

            locale = "en-US";
            localeEnum = locale.ToEnum(LocaleType.EnGb);
            Assert.Equal(LocaleType.EnGb, localeEnum);
        }

        [Theory]
        [InlineData("")]
        [InlineData("9")]
        [InlineData("appsettings.json")]
        public void GetEnumSetting_InvalidEnumValue_ArgumentException(string settingName)
        {
            Assert.Throws<ArgumentException>(() => settingName.ToEnum<DayOfWeek>());
        }

        [Fact]
        public void GetEnumSetting_NotEnumeration_ArgumentException()
        {
            Assert.Throws<InvalidOperationException>(() => "5".ToEnum<int>());
        }

        [Theory]
        [InlineData("", DayOfWeek.Tuesday)]
        [InlineData("9", DayOfWeek.Wednesday)]
        [InlineData("appsettings.json", default(DayOfWeek))]
        public void GetEnumSetting_UseDefaultValue_Success(string settingName, DayOfWeek defaultValue)
        {
            Assert.Equal(defaultValue, settingName.ToEnum(defaultValue));
        }

        [Theory]
        [InlineData(DayOfWeek.Friday, "5")]
        [InlineData(DayOfWeek.Saturday, "Saturday")]
        [InlineData(DayOfWeek.Sunday, "Sunday")]
        public void GetEnumSetting_ValidSetting_Success(DayOfWeek expectedValue, string settingName)
        {
            Assert.Equal(expectedValue, settingName.ToEnum<DayOfWeek>());
        }
    }
}