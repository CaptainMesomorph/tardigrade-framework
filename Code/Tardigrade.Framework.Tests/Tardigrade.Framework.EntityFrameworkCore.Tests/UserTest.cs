using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tardigrade.Framework.EntityFrameworkCore.Extensions;
using Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Persistence;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models.Credentials;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests;

public class UserTest : IClassFixture<EntityFrameworkCoreClassFixture>
{
    private readonly EntityFrameworkCoreClassFixture _fixture;
    private readonly ITestOutputHelper _output;
    private readonly IRepository<UserCredential, Guid> _userCredentialRepository;
    private readonly IRepository<User, Guid> _userRepository;

    /// <summary>
    /// Create an instance of this test.
    /// </summary>
    /// <param name="fixture">Class fixture.</param>
    /// <param name="output">Logger.</param>
    /// <exception cref="ArgumentNullException">Parameters are null.</exception>
    public UserTest(EntityFrameworkCoreClassFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        _output = output ?? throw new ArgumentNullException(nameof(output));

        _userCredentialRepository = _fixture.GetService<IRepository<UserCredential, Guid>>();
        _userRepository = _fixture.GetService<IRepository<User, Guid>>();

        // Get the current project's directory to store the create script.
        DirectoryInfo? binDirectory =
            Directory.GetParent(Directory.GetCurrentDirectory())?.Parent ??
            Directory.GetParent(Directory.GetCurrentDirectory());
        DirectoryInfo? projectDirectory = binDirectory?.Parent ?? binDirectory;
        string scriptDirectory = projectDirectory?.FullName ?? "c:\\temp";

        // Create and store SQL script for the test database.
        _fixture.GetService<DbContext>().GenerateCreateScript($"{scriptDirectory}\\Scripts\\TestDataCreateScript.sql");
        _fixture.PopulateDataStore();
    }

    [Fact]
    public void CreateMultiple_NewObjects_Success()
    {
        // Arrange.
        int count = _userRepository.Count();
        _output.WriteLine($"Total number of users is {count}.");
        IEnumerable<User> multipleOriginals = DataFactory.Users;
        int newObjectsCount = multipleOriginals.Count();

        // Act.
        IEnumerable<User> multipleCreated =
            ((IBulkRepository<User>)_userRepository).CreateBulk(multipleOriginals).ToList();
        var userCredentials = new List<UserCredential>();

        foreach (User user in multipleCreated)
        {
            foreach (UserCredential userCredential in user.UserCredentials)
            {
                userCredential.UserId = user.Id;
            }

            userCredentials.AddRange(user.UserCredentials);
        }

        _ = ((IBulkRepository<UserCredential>)_userCredentialRepository).CreateBulk(userCredentials).ToList();
        _output.WriteLine($"Created {multipleCreated.Count()} new users.");

        // Assert.
        int updatedCount = _userRepository.Count();
        _output.WriteLine($"Total number of users has been increased to {updatedCount}.");
        Assert.Equal(count + newObjectsCount, updatedCount);

        ((IBulkRepository<User>)_userRepository).DeleteBulk(multipleCreated);
    }

    [Fact]
    public void Crud_NewObject_Success()
    {
        int originalCount = _userRepository.Count();
        _output.WriteLine($"Total number of users before executing CRUD operation is {originalCount}.");

        // Create.
        User original = DataFactory.User;
        User created = _userRepository.Create(original);
        _output.WriteLine($"Created new user: {created.Email}");
        Assert.Equal(original.Id, created.Id);

        // Retrieve single.
        User retrieved = _userRepository.Retrieve(created.Id);
        _output.WriteLine($"Retrieved newly created user: {retrieved.Email}");
        Assert.Equal(created.Id, retrieved.Id);

        // Update.
        retrieved.ModifiedBy = "muppet";
        _userRepository.Update(retrieved);
        User updated = _userRepository.Retrieve(retrieved.Id);
        _output.WriteLine($"Updated the ModifiedBy property of the newly created user to: {updated.ModifiedBy}");
        Assert.Equal(retrieved.Id, updated.Id);
        Assert.Equal("muppet", updated.ModifiedBy);

        // Delete.
        _userRepository.Delete(created);
        bool userExists = _userRepository.Exists(created.Id);
        _output.WriteLine($"Successfully deleted user {created.Email}.");
        Assert.False(userExists);
        int currentCount = _userRepository.Count();
        _output.WriteLine($"Total number of users after executing CRUD operation is {currentCount}.");
        Assert.Equal(originalCount, currentCount);

        // TODO: 24.02.2021 This issue needs further investigation.
        // For reasons unknown, the query filter to ignore soft deleted records does not seem to be applied with
        // the retrieve operation. May be an issue with the current transaction not detaching the deleted record
        // from the entity state.
        //User deleted = userRepository.Retrieve(created.Id);
        //output.WriteLine($"Successfully deleted user {created.Id} - {deleted == null}.");
        //Assert.Null(deleted);
    }

    [Fact]
    public void Delete_NewObject_Success()
    {
        // Arrange.
        User original = DataFactory.User;
        User created = _userRepository.Create(original);
        _output.WriteLine($"User to delete:\n{created.ToJson()}");
        Assert.Equal(original.Id, created.Id);

        // Act.
        _userRepository.Delete(created);

        // Assert.
        bool userExists = _userRepository.Exists(original.Id);
        Assert.False(userExists);
        bool userCredentialExists = _userCredentialRepository.Exists(original.UserCredentials.First().Id);
        Assert.False(userCredentialExists);

        _output.WriteLine($"Successfully deleted user {original.Id}.");
    }

    [Fact]
    public void Exists_ExistingObject_Success()
    {
        // Arrange.

        // Act.
        bool exists = _userRepository.Exists(_fixture.ReferenceUser.Id);

        // Assert.
        _output.WriteLine($"Existing user found - {exists}.");
        Assert.True(exists);
    }

    [Fact]
    public void RetrieveAll_ExistingObjects_Success()
    {
        // Arrange.

        // Act.
        IEnumerable<User> items = _userRepository.Retrieve().ToList();
        _output.WriteLine("All users:");

        foreach (User item in items)
        {
            _output.WriteLine($">>>> {item.ToJson()}");
        }

        // Assert.
        Assert.True(items.Any());
    }

    [Fact]
    public void RetrieveAllEagerLoading_ExistingObjects_Success()
    {
        // Arrange.

        // Act.
        IEnumerable<User> items = _userRepository
            .Retrieve(includes: u => u.UserCredentials.Select(uc => uc.Credentials))
            .ToList();
        _output.WriteLine("All eager loaded users:");

        // Assert.
        foreach (User item in items)
        {
            _output.WriteLine($">>>> {item.ToJson()}");
            Assert.False(item.UserCredentials.All(uc => uc.Credentials.All(c => c.Name == null)));
        }
    }

    [Fact]
    public void RetrieveEagerLoading_ExistingObject_Success()
    {
        // Arrange.

        // Act.
        User retrieved = _userRepository.Retrieve(
            _fixture.ReferenceUser.Id,
            u => u.UserCredentials.Select(uc => uc.Credentials.Select(c => c.Issuers)));
        _output.WriteLine($"Eager loaded existing user:\n{retrieved.ToJson()}");

        // Assert.
        Assert.NotNull(retrieved);
        string? originalCredentialIssuerName = _fixture.ReferenceUser
            .UserCredentials.OrderByDescending(uc => uc.CreatedDate).FirstOrDefault()?
            .Credentials.OrderByDescending(c => c.CreatedDate).FirstOrDefault()?
            .Issuers.OrderByDescending(ci => ci.CreatedDate).FirstOrDefault()?.Name;
        string? retrievedCredentialIssuerName = retrieved
            .UserCredentials.OrderByDescending(uc => uc.CreatedDate).FirstOrDefault()?
            .Credentials.OrderByDescending(c => c.CreatedDate).FirstOrDefault()?
            .Issuers.OrderByDescending(ci => ci.CreatedDate).FirstOrDefault()?.Name;
        Assert.Equal(originalCredentialIssuerName, retrievedCredentialIssuerName);
    }

    [Fact]
    public void RetrieveLatest_ExistingObject_Success()
    {
        // Arrange.
        User original = DataFactory.User;
        original.CreatedDate = DateTime.Now;
        User created = _userRepository.Create(original);
        _output.WriteLine($"Created new user:\n{created.ToJson()}");

        // Act.
        User? latest = _userRepository
            .Retrieve(sortCondition: p => p.OrderByDescending(o => o.CreatedDate))
            .FirstOrDefault();
        _output.WriteLine($"Most recently created user:\n{latest.ToJson()}");

        // Assert.
        Assert.NotNull(latest);
        Assert.Equal(created.Id, latest?.Id);

        _userRepository.Delete(created);
    }
}