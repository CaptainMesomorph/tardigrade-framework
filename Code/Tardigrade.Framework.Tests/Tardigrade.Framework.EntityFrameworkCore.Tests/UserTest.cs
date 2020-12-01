using Newtonsoft.Json;
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
        private readonly IRepository<User, Guid> repository;

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

            repository = this.fixture.Container.GetService<IRepository<User, Guid>>();
        }

        // TODO: Investigate why CreateBulk does not create associated child entities.
        //[Theory]
        //[InlineData(5)]
        //public void CreateMultiple_NewObjects_Success(uint newObjectsCount)
        //{
        //    // Arrange.
        //    int count = repository.Count();
        //    output.WriteLine($"Total number of users is {count}.");
        //    IEnumerable<User> multipleOriginals = DataFactory.CreateUsers(newObjectsCount);

        //    // Act.
        //    IEnumerable<User> multipleCreated = ((IBulkRepository<User>)repository).CreateBulk(multipleOriginals);
        //    output.WriteLine($"Created {multipleCreated.Count()} new users.");

        //    // Assert.
        //    int updatedCount = repository.Count();
        //    output.WriteLine($"Total number of users has been increased to {updatedCount}.");
        //    Assert.Equal(count + newObjectsCount, updatedCount);
        //}

        [Fact]
        public void Crud_NewObject_Success()
        {
            int originalCount = repository.Count();
            output.WriteLine($"Total number of users before executing CRUD operation is {originalCount}.");

            // Create.
            User original = DataFactory.CreateUser();
            User created = repository.Create(original);
            output.WriteLine($"Created new user:\n{Output(created)}");
            Assert.Equal(original.Id, created.Id);

            // Retrieve single.
            User retrieved = repository.Retrieve(created.Id);
            output.WriteLine($"Retrieved newly created user:\n{Output(retrieved)}");
            Assert.Equal(created.Id, retrieved.Id);

            // Update.
            retrieved.ModifiedBy = "muppet";
            repository.Update(retrieved);
            User updated = repository.Retrieve(retrieved.Id);
            output.WriteLine($"Updated the ModifiedBy property of the newly created user:\n{Output(updated)}");
            Assert.Equal(retrieved.Id, updated.Id);
            Assert.Equal("muppet", updated.ModifiedBy);

            // Delete.
            repository.Delete(created);
            User deleted = repository.Retrieve(created.Id);
            output.WriteLine($"Successfully deleted user {created.Id} - {deleted == null}.");
            Assert.Null(deleted);
            int currentCount = repository.Count();
            output.WriteLine($"Total number of users after executing CRUD operation is {currentCount}.");
            Assert.Equal(originalCount, currentCount);
        }

        [Fact]
        public void Exists_ExistingObject_Success()
        {
            // Arrange.

            // Act.
            bool exists = repository.Exists(fixture.ReferenceUser.Id);

            // Assert.
            output.WriteLine($"Existing user found - {exists}.");
            Assert.True(exists);
        }

        [Fact]
        public void RetrieveAll_ExistingObjects_Success()
        {
            // Arrange.

            // Act.
            IEnumerable<User> items = repository.Retrieve().ToList();
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
            IEnumerable<User> items = repository
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
            User retrieved = repository.Retrieve(
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
            User created = repository.Create(original);
            output.WriteLine($"Created new user:\n{Output(created)}");

            // Act.
            User latest = repository
                .Retrieve(sortCondition: p => p.OrderByDescending(o => o.CreatedDate))
                .FirstOrDefault();
            output.WriteLine($"Most recently created user:\n{Output(latest)}");

            // Assert.
            Assert.NotNull(latest);
            Assert.Equal(created.Id, latest.Id);

            repository.Delete(created);
        }
    }
}