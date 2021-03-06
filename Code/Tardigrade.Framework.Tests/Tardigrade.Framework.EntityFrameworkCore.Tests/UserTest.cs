﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp;
using Tardigrade.Framework.Persistence;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests
{
    public class UserTest : IClassFixture<UnitTestFixture>
    {
        private readonly UnitTestFixture fixture;
        private readonly ITestOutputHelper output;
        private readonly IRepository<UserCredential, Guid> userCredentialRepository;
        private readonly IRepository<User, Guid> userRepository;

        private static string Output(User obj)
        {
            var options = new JsonSerializerSettings
            {
                //ContractResolver = new IgnoreVirtualPropertyResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(obj, options);
        }

        public UserTest(UnitTestFixture fixture, ITestOutputHelper output)
        {
            this.output = output;
            this.fixture = fixture;

            userCredentialRepository = this.fixture.Container.GetService<IRepository<UserCredential, Guid>>();
            userRepository = this.fixture.Container.GetService<IRepository<User, Guid>>();
        }

        [Theory]
        [InlineData(5)]
        public void CreateMultiple_NewObjects_Success(uint newObjectsCount)
        {
            // Arrange.
            int count = userRepository.Count();
            output.WriteLine($"Total number of users is {count}.");
            IEnumerable<User> multipleOriginals = DataFactory.CreateUsers(newObjectsCount);

            // Act.
            IEnumerable<User> multipleCreated =
                ((IBulkRepository<User>)userRepository).CreateBulk(multipleOriginals).ToList();
            var userCredentials = new List<UserCredential>();

            foreach (User user in multipleCreated)
            {
                foreach (UserCredential userCredential in user.UserCredentials)
                {
                    userCredential.UserId = user.Id;
                }

                userCredentials.AddRange(user.UserCredentials);
            }

            _ = ((IBulkRepository<UserCredential>)userCredentialRepository).CreateBulk(userCredentials).ToList();
            output.WriteLine($"Created {multipleCreated.Count()} new users.");

            // Assert.
            int updatedCount = userRepository.Count();
            output.WriteLine($"Total number of users has been increased to {updatedCount}.");
            Assert.Equal(count + newObjectsCount, updatedCount);

            ((IBulkRepository<User>)userRepository).DeleteBulk(multipleCreated);
        }

        [Fact]
        public void Crud_NewObject_Success()
        {
            int originalCount = userRepository.Count();
            output.WriteLine($"Total number of users before executing CRUD operation is {originalCount}.");

            // Create.
            User original = DataFactory.CreateUser();
            User created = userRepository.Create(original);
            output.WriteLine($"Created new user:\n{Output(created)}");
            Assert.Equal(original.Id, created.Id);

            // Retrieve single.
            User retrieved = userRepository.Retrieve(created.Id);
            output.WriteLine($"Retrieved newly created user:\n{Output(retrieved)}");
            Assert.Equal(created.Id, retrieved.Id);

            // Update.
            retrieved.ModifiedBy = "muppet";
            userRepository.Update(retrieved);
            User updated = userRepository.Retrieve(retrieved.Id);
            output.WriteLine($"Updated the ModifiedBy property of the newly created user:\n{Output(updated)}");
            Assert.Equal(retrieved.Id, updated.Id);
            Assert.Equal("muppet", updated.ModifiedBy);

            // Delete.
            userRepository.Delete(created);
            User deleted = userRepository.Retrieve(created.Id);
            output.WriteLine($"Successfully deleted user {created.Id} - {deleted == null}.");
            Assert.Null(deleted);
            int currentCount = userRepository.Count();
            output.WriteLine($"Total number of users after executing CRUD operation is {currentCount}.");
            Assert.Equal(originalCount, currentCount);
        }

        [Fact]
        public void Exists_ExistingObject_Success()
        {
            // Arrange.

            // Act.
            bool exists = userRepository.Exists(fixture.ReferenceUser.Id);

            // Assert.
            output.WriteLine($"Existing user found - {exists}.");
            Assert.True(exists);
        }

        [Fact]
        public void RetrieveAll_ExistingObjects_Success()
        {
            // Arrange.

            // Act.
            IEnumerable<User> items = userRepository.Retrieve().ToList();
            output.WriteLine("All users:");

            foreach (User item in items)
            {
                output.WriteLine($">>>> {Output(item)}");
            }

            // Assert.
            Assert.True(items.Any());
        }

        [Fact]
        public void RetrieveAllEagerLoading_ExistingObjects_Success()
        {
            // Arrange.

            // Act.
            IEnumerable<User> items = userRepository
                .Retrieve(includes: u => u.UserCredentials.Select(uc => uc.Credentials))
                .ToList();
            output.WriteLine("All eager loaded users:");

            // Assert.
            foreach (User item in items)
            {
                output.WriteLine($">>>> {Output(item)}");
                Assert.False(item.UserCredentials.All(uc => uc.Credentials.All(c => c.Name == null)));
            }
        }

        [Fact]
        public void RetrieveEagerLoading_ExistingObject_Success()
        {
            // Arrange.

            // Act.
            User retrieved = userRepository.Retrieve(
                fixture.ReferenceUser.Id,
                u => u.UserCredentials.Select(uc => uc.Credentials.Select(c => c.Issuers)));
            output.WriteLine($"Eager loaded existing user:\n{Output(retrieved)}");

            // Assert.
            Assert.NotNull(retrieved);
            Assert.Equal(
                fixture.ReferenceUser.UserCredentials.FirstOrDefault()?.Credentials.FirstOrDefault()?
                    .Issuers.FirstOrDefault()?.Name,
                retrieved.UserCredentials.FirstOrDefault()?.Credentials.FirstOrDefault()?
                    .Issuers.FirstOrDefault()?.Name);
        }

        [Fact]
        public void RetrieveLatest_ExistingObject_Success()
        {
            // Arrange.
            User original = DataFactory.CreateUser();
            original.CreatedDate = DateTime.Now;
            User created = userRepository.Create(original);
            output.WriteLine($"Created new user:\n{Output(created)}");

            // Act.
            User latest = userRepository
                .Retrieve(sortCondition: p => p.OrderByDescending(o => o.CreatedDate))
                .FirstOrDefault();
            output.WriteLine($"Most recently created user:\n{Output(latest)}");

            // Assert.
            Assert.NotNull(latest);
            Assert.Equal(created.Id, latest.Id);

            userRepository.Delete(created);
        }
    }
}