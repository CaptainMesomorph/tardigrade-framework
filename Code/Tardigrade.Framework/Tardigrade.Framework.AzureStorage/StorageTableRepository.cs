using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.AzureStorage.Models;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.AzureStorage
{
    /// <summary>
    /// Repository layer that is based on Azure Storage Tables.
    /// <see cref="IRepository{T, PK}"/>
    /// </summary>
    public class StorageTableRepository<T, PK> : StorageTableReadOnlyRepository<T, PK>, IRepository<T, PK>
        where T : ITableEntity, new()
        where PK : ITableKey
    {
        /// <summary>
        /// <see cref="StorageTableReadOnlyRepository{T, PK}.StorageTableReadOnlyRepository(string, string)"/>
        /// </summary>
        public StorageTableRepository(string connectionString, string tableName) : base(connectionString, tableName)
        {
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Create(T)"/>
        /// </summary>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual T Create(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            // Create the TableOperation that inserts the object.
            TableOperation operation = TableOperation.Insert(obj, true);

            TableResult result;

            try
            {
                // Execute the insert operation.
                result = Table.ExecuteAsync(operation).GetAwaiter().GetResult();
            }
            // https://stackoverflow.com/questions/44799524/azure-table-storage-throwing-exception-on-insert-409-conflict
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Create failed; error creating object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} in Azure Storage Table {Table.Name}.",
                    e);
            }

            if (result.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }
            else
            {
                throw new RepositoryException(
                    $"Create failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} for Azure Storage Table {Table.Name}.");
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task<T> CreateAsync(
            T obj,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            // Create the TableOperation that inserts the object.
            TableOperation operation = TableOperation.Insert(obj, true);

            TableResult result;

            try
            {
                // Execute the insert operation.
                result = await Table.ExecuteAsync(operation);
            }
            // https://stackoverflow.com/questions/44799524/azure-table-storage-throwing-exception-on-insert-409-conflict
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }

            if (result.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {Table.Name}.");
            }
            else
            {
                throw new RepositoryException(
                    $"Create failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} for Azure Storage Table {Table.Name}.");
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual IEnumerable<T> CreateBulk(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task<IEnumerable<T>> CreateBulkAsync(
            IEnumerable<T> objs,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Delete(T)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The obj parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        public virtual void Delete(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!Exists(obj.PartitionKey, obj.RowKey))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            // Create the Delete TableOperation.
            TableOperation operation = TableOperation.Delete(obj);

            try
            {
                // Execute the operation.
                TableResult result = Table.ExecuteAsync(operation).GetAwaiter().GetResult();

                if (result.HttpStatusCode != (int)HttpStatusCode.OK)
                {
                    throw new RepositoryException(
                        $"Delete failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} in Azure Storage Table {Table.Name}.");
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Delete failed; error deleting object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} from Azure Storage Table {Table.Name}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The obj parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        public virtual async Task DeleteAsync(T obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!await ExistsAsync(obj.PartitionKey, obj.RowKey))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            // Create the Delete TableOperation.
            TableOperation operation = TableOperation.Delete(obj);

            // Execute the operation.
            TableResult result = await Table.ExecuteAsync(operation);

            if (result.HttpStatusCode != (int)HttpStatusCode.OK)
            {
                throw new RepositoryException(
                    $"Delete failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} in Azure Storage Table {Table.Name}.");
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.DeleteBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual void DeleteBulk(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.DeleteBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task DeleteBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Update(T)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The obj parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual void Update(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!Exists(obj.PartitionKey, obj.RowKey))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            try
            {
                // Create the InsertOrReplace TableOperation.
                TableOperation insertOrReplaceOperation = TableOperation.Replace(obj);

                // Execute the operation.
                Table.ExecuteAsync(insertOrReplaceOperation).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Update failed; error updating object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} from Azure Storage Table {Table.Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The obj parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task UpdateAsync(T obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (!await ExistsAsync(obj.PartitionKey, obj.RowKey))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {Table.Name}.");
            }

            // Create the InsertOrReplace TableOperation.
            TableOperation insertOrReplaceOperation = TableOperation.Replace(obj);

            // Execute the operation.
            await Table.ExecuteAsync(insertOrReplaceOperation);
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.UpdateBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual void UpdateBulk(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.UpdateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task UpdateBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}