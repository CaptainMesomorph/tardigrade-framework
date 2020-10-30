using Microsoft.Azure.Cosmos.Table;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.AzureStorage.Models;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Helpers;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.AzureStorage.Tables
{
    /// <summary>
    /// Repository layer that is based on Azure Storage Tables.
    /// <see cref="IRepository{TEntity, TKey}"/>
    /// </summary>
    public class Repository<TEntity, TKey> : ReadOnlyRepository<TEntity, TKey>, IRepository<TEntity, TKey>
        where TEntity : ITableEntity, new()
        where TKey : ITableKey
    {
        /// <summary>
        /// <see cref="ReadOnlyRepository{TEntity, TKey}"/>
        /// </summary>
        public Repository(string connectionString, string tableName) : base(connectionString, tableName)
        {
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.Create(TEntity)"/>
        /// </summary>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual TEntity Create(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Create the TableOperation that inserts the object.
            TableOperation operation = TableOperation.Insert(item, true);

            TableResult result;

            try
            {
                // Execute the insert operation.
                result = AsyncHelper.RunSync(() => Table.ExecuteAsync(operation));
            }
            // https://stackoverflow.com/questions/44799524/azure-table-storage-throwing-exception-on-insert-409-conflict
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Create failed; error creating object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} in Azure Storage Table {Table.Name}.",
                    e);
            }

            if (result.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                item = (TEntity)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }
            else
            {
                throw new RepositoryException(
                    $"Create failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} for Azure Storage Table {Table.Name}.");
            }

            return item;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.CreateAsync(TEntity, CancellationToken)"/>
        /// </summary>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task<TEntity> CreateAsync(
            TEntity item,
            CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            // Create the TableOperation that inserts the object.
            TableOperation operation = TableOperation.Insert(item, true);

            TableResult result;

            try
            {
                // Execute the insert operation.
                result = await Table.ExecuteAsync(operation, cancellationToken);
            }
            // https://stackoverflow.com/questions/44799524/azure-table-storage-throwing-exception-on-insert-409-conflict
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }

            if (result.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                item = (TEntity)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }
            else
            {
                throw new RepositoryException(
                    $"Create failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} for Azure Storage Table {Table.Name}.");
            }

            return item;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.Delete(TEntity)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        public virtual void Delete(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!Exists(item.PartitionKey, item.RowKey))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            // Create the Delete TableOperation.
            TableOperation operation = TableOperation.Delete(item);

            try
            {
                // Execute the operation.
                TableResult result = AsyncHelper.RunSync(() => Table.ExecuteAsync(operation));

                if (result.HttpStatusCode != (int)HttpStatusCode.OK)
                {
                    throw new RepositoryException(
                        $"Delete failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} in Azure Storage Table {Table.Name}.");
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Delete failed; error deleting object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} from Azure Storage Table {Table.Name}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.DeleteAsync(TEntity, CancellationToken)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        public virtual async Task DeleteAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!await ExistsAsync(item.PartitionKey, item.RowKey))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            // Create the Delete TableOperation.
            TableOperation operation = TableOperation.Delete(item);

            // Execute the operation.
            TableResult result = await Table.ExecuteAsync(operation, cancellationToken);

            if (result.HttpStatusCode != (int)HttpStatusCode.OK)
            {
                throw new RepositoryException(
                    $"Delete failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} in Azure Storage Table {Table.Name}.");
            }
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.Update(TEntity)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual void Update(TEntity item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!Exists(item.PartitionKey, item.RowKey))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            try
            {
                // Create the InsertOrReplace TableOperation.
                TableOperation insertOrReplaceOperation = TableOperation.Replace(item);

                // Execute the operation.
                AsyncHelper.RunSync(() => Table.ExecuteAsync(insertOrReplaceOperation));
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Update failed; error updating object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} from Azure Storage Table {Table.Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.UpdateAsync(TEntity, CancellationToken)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!await ExistsAsync(item.PartitionKey, item.RowKey))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(TEntity).Name} with Partition Key {item.PartitionKey} and Row Key{item.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            // Create the InsertOrReplace TableOperation.
            TableOperation insertOrReplaceOperation = TableOperation.Replace(item);

            // Execute the operation.
            await Table.ExecuteAsync(insertOrReplaceOperation, cancellationToken);
        }
    }
}