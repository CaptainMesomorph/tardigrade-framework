using System;
using Tardigrade.Framework.AzureStorage.Tables;
using Tardigrade.Framework.AzureStorage.Tests.Models;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Patterns.DependencyInjection;
using Tardigrade.Framework.Persistence;
using Xunit;

namespace Tardigrade.Framework.AzureStorage.Tests
{
    public class StorageTableRepositoryTest : IClassFixture<ServiceProviderFixture>
    {
        private readonly IServiceContainer container;

        public StorageTableRepositoryTest(ServiceProviderFixture fixture)
        {
            container = fixture.Container;
        }

        [Theory]
        [InlineData("DefaultEndpointsProtocol=https;AccountName=tikforcedevstorage;AccountKey=U10Q5T6AsbPyLS3mJpJGRLaU3t7ETPbBraeAK+aG1532BdTQnvV44mKXgS1VwKqzszOELiB57kjphJ/GBb0/AA==", "MockTable")]
        public void Constructor_ConnectionStringNoEndpoints_Success(string storageConnectionString, string tableName)
        {
            // Arrange.
            IRepository<FakeTableEntity, FakeTableKey> repository =
                new Repository<FakeTableEntity, FakeTableKey>(storageConnectionString, tableName);
            var entity = new FakeTableEntity() { PartitionKey = "Mock", RowKey = Guid.NewGuid().ToString() };

            // Act.
            FakeTableEntity createdEntity = repository.Create(entity);

            // Assert.
            Assert.Equal(entity, createdEntity);
        }

        [Fact]
        public void Constructor_Instantiate_Success()
        {
            // Arrange.
            var repository = container.GetService<IRepository<FakeTableEntity, FakeTableKey>>();
            var entity = new FakeTableEntity() { PartitionKey = "Mock", RowKey = Guid.NewGuid().ToString() };

            // Act.
            FakeTableEntity createdEntity = repository.Create(entity);

            // Assert.
            Assert.Equal(entity, createdEntity);
        }

        [Theory]
        [InlineData("blah", "MockTable")]
        [InlineData("name=value", "MockTable")]
        [InlineData("DefaultEndpointsProtocol=https;AccountName=IncorrectName;AccountKey=U10Q5T6AsbPyLS3mJpJGRLaU3t7ETPbBraeAK+aG1532BdTQnvV44mKXgS1VwKqzszOELiB57kjphJ/GBb0/AA==;BlobEndpoint=https://tikforcedevstorage.blob.core.windows.net/;TableEndpoint=https://tikforcedevstorage.table.core.windows.net/;QueueEndpoint=https://tikforcedevstorage.queue.core.windows.net/;FileEndpoint=https://tikforcedevstorage.file.core.windows.net/", "MockTable")]
        [InlineData("DefaultEndpointsProtocol=https;AccountName=tikforcedevstorage;AccountKey=IncorrectPassword;BlobEndpoint=https://tikforcedevstorage.blob.core.windows.net/;TableEndpoint=https://tikforcedevstorage.table.core.windows.net/;QueueEndpoint=https://tikforcedevstorage.queue.core.windows.net/;FileEndpoint=https://tikforcedevstorage.file.core.windows.net/", "MockTable")]
        [InlineData("DefaultEndpointsProtocol=https;AccountName=tikforcedevstorage;AccountKey=U10Q5T6AsbPyLS3mJpJGRLaU3t7ETPbBraeAK+aG1532BdTQnvV44mKXgS1VwKqzszOELiB57kjphJ/GBb0/AA==;BlobEndpoint=https://tikforcedevstorage.blob.core.windows.net/;TableEndpoint=https://incorrect.table.core.windows.net/;QueueEndpoint=https://tikforcedevstorage.queue.core.windows.net/;FileEndpoint=https://tikforcedevstorage.file.core.windows.net/", "MockTable")]
        public void Constructor_InvalidConnectionString_FormatException(string storageConnectionString, string tableName)
        {
            // Arrange.

            // Act.
            Action actual = (() => _ = new Repository<FakeTableEntity, FakeTableKey>(storageConnectionString, tableName));

            // Assert.
            Assert.Throws<FormatException>(actual);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, "")]
        [InlineData(null, "     ")]
        [InlineData("", null)]
        [InlineData("    ", null)]
        public void Constructor_NullOrEmptyParameters_ArgumentNullException(string storageConnectionString, string tableName)
        {
            // Arrange.

            // Act.
            Action actual = (() => _ = new Repository<FakeTableEntity, FakeTableKey>(storageConnectionString, tableName));

            // Assert.
            Assert.Throws<ArgumentNullException>(actual);
        }

        [Fact]
        public void Create_ExistingObject_AlreadyExistsException()
        {
            // Arrange.
            var repository = container.GetService<IRepository<FakeTableEntity, FakeTableKey>>();
            var entity = new FakeTableEntity() { PartitionKey = "Mock", RowKey = Guid.NewGuid().ToString() };
            repository.Create(entity);

            // Act.
            Action actual = (() => repository.Create(entity));

            // Assert.
            Assert.Throws<AlreadyExistsException>(actual);
        }
    }
}