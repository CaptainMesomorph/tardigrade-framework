﻿using System;
using System.Reflection;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Testing;
using Tardigrade.Shared.Tests;
using Xunit;

namespace Tardigrade.Framework.Tests;

public class ApplicationConfigurationJsonTest : ApplicationConfigurationTest
{
    public ApplicationConfigurationJsonTest()
        : base(new UnitTestApplicationConfiguration(Assembly.GetExecutingAssembly()))
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
        Assert.Throws<FormatException>(() => Config.GetAsEnum<DayOfWeek>(settingName));
    }

    [Theory]
    [InlineData("Test.NullValue")]
    public void GetIntSetting_NullValue_FormatException(string settingName)
    {
        Assert.Throws<FormatException>(() => Config.GetAsInt(settingName));
    }

    [Theory]
    [InlineData("appsettings.json", "Test.Source")]
    [InlineData("secrets.json", "Test.Source.Secret")]
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

    [Theory]
    [InlineData("Test.NullValue")]
    public void GetUriSetting_NullValue_FormatException(string settingName)
    {
        Assert.Throws<UriFormatException>(() => Config.GetAsUri(settingName));
    }
}