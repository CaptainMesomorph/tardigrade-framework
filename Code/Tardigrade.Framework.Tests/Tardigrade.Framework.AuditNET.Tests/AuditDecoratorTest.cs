using Audit.Core;
using System;
using Tardigrade.Framework.AuditNET.AzureStorageQueue.DataProviders;
using Tardigrade.Framework.AuditNET.Tests.SetUp;
using Tardigrade.Framework.AzureStorage.Configurations;
using Tardigrade.Framework.Helpers;
using Tardigrade.Framework.Services;
using Tardigrade.Shared.Tests.Models.Credentials;
using Tardigrade.Shared.Tests.Models.Passwords;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.AuditNET.Tests;

public class AuditDecoratorTest : IClassFixture<AuditClassFixture>
{
    private readonly ITestOutputHelper _output;
    private readonly Password _password;
    private readonly IObjectService<Password, Guid> _passwordService;
    private readonly User _user;
    private readonly IObjectService<User, Guid> _userService;

    public AuditDecoratorTest(AuditClassFixture fixture, ITestOutputHelper output)
    {
        _output = output;
        _password = fixture.ReferencePassword;
        _passwordService =
            fixture.GetService<IObjectService<Password, Guid>>() ?? throw new InvalidOperationException();
        _user = fixture.ReferenceUser;
        _userService = fixture.GetService<IObjectService<User, Guid>>() ?? throw new InvalidOperationException();

        IAzureStorageConfiguration config =
            fixture.GetService<IAzureStorageConfiguration>() ?? throw new InvalidOperationException();
        Configuration.DataProvider = new AzureQueueDataProvider(config.AzureStorageConnectionString, "audit-tests");
        Configuration.JsonSettings = JsonHelper.SerializerOptions;
    }

    [Fact]
    public void CreatePassword_Valid_Success()
    {
        // Arrange.

        // Act.
        Password password = _passwordService.Create(_password);

        // Assert.
        Assert.Equal(_password, password);

        _output.WriteLine("Manually check Azure Queue to confirm password create operation successfully decorated.");
    }

    [Fact]
    public void CreateUser_Valid_Success()
    {
        // Arrange.

        // Act.
        User user = _userService.Create(_user);

        // Assert.
        Assert.Equal(_user, user);

        _output.WriteLine("Manually check Azure Queue to confirm user create operation successfully decorated.");
    }
}