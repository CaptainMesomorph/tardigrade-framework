using System;
using Tardigrade.Framework.Extensions;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ExtensionTest
    {
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