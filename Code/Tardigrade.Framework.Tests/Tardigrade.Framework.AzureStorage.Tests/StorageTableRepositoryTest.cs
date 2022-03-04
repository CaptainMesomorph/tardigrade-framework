using System;
using Tardigrade.Framework.AzureStorage.Tables;
using Tardigrade.Framework.AzureStorage.Tests.Models;
using Tardigrade.Framework.AzureStorage.Tests.SetUp;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;
using Xunit;

namespace Tardigrade.Framework.AzureStorage.Tests;

public class StorageTableRepositoryTest : IClassFixture<AzureStorageClassFixture>
{
    private readonly IRepository<FakeTableEntity, FakeTableKey> _repository;

    public StorageTableRepositoryTest(AzureStorageClassFixture fixture)
    {
        _repository =
            fixture.GetService<IRepository<FakeTableEntity, FakeTableKey>>() ?? throw new InvalidOperationException();
    }

    [Theory]
    [InlineData("DefaultEndpointsProtocol=https;AccountName=tikforcedevstorage;AccountKey=U10Q5T6AsbPyLS3mJpJGRLaU3t7ETPbBraeAK+aG1532BdTQnvV44mKXgS1VwKqzszOELiB57kjphJ/GBb0/AA==", "MockTable")]
    public void Constructor_ConnectionStringNoEndpoints_Success(string connectionString, string tableName)
    {
        // Arrange.
        IRepository<FakeTableEntity, FakeTableKey> repository =
            new Repository<FakeTableEntity, FakeTableKey>(connectionString, tableName);
        var entity = new FakeTableEntity { PartitionKey = "Mock", RowKey = Guid.NewGuid().ToString() };

        // Act.
        FakeTableEntity createdEntity = repository.Create(entity);

        // Assert.
        Assert.Equal(entity, createdEntity);
    }

    [Fact]
    public void Constructor_Instantiate_Success()
    {
        // Arrange.
        var entity = new FakeTableEntity { PartitionKey = "Mock", RowKey = Guid.NewGuid().ToString() };

        // Act.
        FakeTableEntity createdEntity = _repository.Create(entity);

        // Assert.
        Assert.Equal(entity, createdEntity);
    }

    [Theory]
    [InlineData("blah", "MockTable")]
    [InlineData("name=value", "MockTable")]
    [InlineData("DefaultEndpointsProtocol=https;AccountName=IncorrectName;AccountKey=U10Q5T6AsbPyLS3mJpJGRLaU3t7ETPbBraeAK+aG1532BdTQnvV44mKXgS1VwKqzszOELiB57kjphJ/GBb0/AA==;BlobEndpoint=https://tikforcedevstorage.blob.core.windows.net/;TableEndpoint=https://tikforcedevstorage.table.core.windows.net/;QueueEndpoint=https://tikforcedevstorage.queue.core.windows.net/;FileEndpoint=https://tikforcedevstorage.file.core.windows.net/", "MockTable")]
    [InlineData("DefaultEndpointsProtocol=https;AccountName=tikforcedevstorage;AccountKey=IncorrectPassword;BlobEndpoint=https://tikforcedevstorage.blob.core.windows.net/;TableEndpoint=https://tikforcedevstorage.table.core.windows.net/;QueueEndpoint=https://tikforcedevstorage.queue.core.windows.net/;FileEndpoint=https://tikforcedevstorage.file.core.windows.net/", "MockTable")]
    [InlineData("DefaultEndpointsProtocol=https;AccountName=tikforcedevstorage;AccountKey=U10Q5T6AsbPyLS3mJpJGRLaU3t7ETPbBraeAK+aG1532BdTQnvV44mKXgS1VwKqzszOELiB57kjphJ/GBb0/AA==;BlobEndpoint=https://tikforcedevstorage.blob.core.windows.net/;TableEndpoint=https://incorrect.table.core.windows.net/;QueueEndpoint=https://tikforcedevstorage.queue.core.windows.net/;FileEndpoint=https://tikforcedevstorage.file.core.windows.net/", "MockTable")]
    public void Constructor_InvalidConnectionString_FormatException(string connectionString, string tableName)
    {
        // Arrange.

        // Act.
        void Actual() => _ = new Repository<FakeTableEntity, FakeTableKey>(connectionString, tableName);

        // Assert.
        Assert.Throws<FormatException>(Actual);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "")]
    [InlineData(null, "     ")]
    [InlineData("", null)]
    [InlineData("    ", null)]
    public void Constructor_NullOrEmptyParameters_ArgumentNullException(string connectionString, string tableName)
    {
        // Arrange.

        // Act.
        void Actual() => _ = new Repository<FakeTableEntity, FakeTableKey>(connectionString, tableName);

        // Assert.
        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Create_ExistingObject_AlreadyExistsException()
    {
        // Arrange.
        var entity = new FakeTableEntity { PartitionKey = "Mock", RowKey = Guid.NewGuid().ToString() };
        _repository.Create(entity);

        // Act.
        void Actual() => _repository.Create(entity);

        // Assert.
        Assert.Throws<AlreadyExistsException>(Actual);
    }
}