using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.AzureStorage.Extensions;
using Tardigrade.Framework.AzureStorage.Models;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.AzureStorage
{
    /// <summary>
    /// Repository layer that is based on Azure Storage Tables.
    /// <see cref="IRepository{T, PK}"/>
    /// </summary>
    public class StorageTableRepository<T, PK> : IRepository<T, PK> where T : ITableEntity, new() where PK : ITableKey
    {
        private readonly CloudTable table;

        /// <summary>
        /// Create an instance of this repository. If the table name does not exist, it will be created.
        /// </summary>
        /// <param name="connectionString">Storage Table connection string.</param>
        /// <param name="tableName">Name of the Storage Table.</param>
        /// <exception cref="ArgumentNullException">Parameters are null or empty.</exception>
        /// <exception cref="FormatException">storageConnectionString is not in a valid format for a connection string.</exception>
        /// <exception cref="FormatException">Storage Account name in storageConnectionString is not recognised.</exception>
        /// <exception cref="FormatException">Storage Account name and key combination in storageConnectionString is invalid.</exception>
        /// <exception cref="FormatException">A Storage Account endpoint in storageConnectionString is invalid.</exception>
        public StorageTableRepository(string connectionString, string tableName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            table = tableClient.GetTableReference(tableName);

            // Create the table if it doesn't exist.
            try
            {
                table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
            }
            catch (StorageException e) when (e?.InnerException is HttpRequestException)
            {
                throw new FormatException($"A Storage Account endpoint in the following connection string is invalid:\n{connectionString}", e);
            }
            catch (StorageException e)
            {
                throw new FormatException($"Storage Account name in the following connection string is not recognised:\n{connectionString}", e);
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Failed to create Azure Storage Table {table.Name} using the following connection string.\n{connectionString}", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task<int> CountAsync(
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
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
                result = table.ExecuteAsync(operation).GetAwaiter().GetResult();
            }
            // https://stackoverflow.com/questions/44799524/azure-table-storage-throwing-exception-on-insert-409-conflict
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException($"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {table.Name}.");
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Create failed; error creating object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} in Azure Storage Table {table.Name}.", e);
            }

            if (result.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException($"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {table.Name}.");
            }
            else
            {
                throw new RepositoryException($"Create failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} for Azure Storage Table {table.Name}.");
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
                result = await table.ExecuteAsync(operation);
            }
            // https://stackoverflow.com/questions/44799524/azure-table-storage-throwing-exception-on-insert-409-conflict
            catch (StorageException e) when (e.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException($"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {table.Name}.");
            }

            if (result.HttpStatusCode == (int)HttpStatusCode.Created)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new AlreadyExistsException($"Create failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} already exists in Azure Storage Table {table.Name}.");
            }
            else
            {
                throw new RepositoryException($"Create failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} for Azure Storage Table {table.Name}.");
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.CreateBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual IEnumerable<T> CreateBulk(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.CreateBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task<IEnumerable<T>> CreateBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Delete(PK)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The id parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        public virtual void Delete(PK id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            TableResult retrieveResult = GetRetrieveTableResult(id.Partition, id.Row);

            if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                // Assign the result to be deleted.
                T deleteEntity = (T)retrieveResult.Result;

                // Create the Delete TableOperation.
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                try
                {
                    // Execute the operation.
                    TableResult deleteResult = table.ExecuteAsync(deleteOperation).GetAwaiter().GetResult();

                    if (deleteResult.HttpStatusCode != (int)HttpStatusCode.OK)
                    {
                        throw new RepositoryException($"Delete failed; error code of {deleteResult.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {id.Partition} and Row Key{id.Row} in Azure Storage Table {table.Name}.");
                    }
                }
                catch (Exception e)
                {
                    throw new RepositoryException($"Delete failed; error deleting object of type {typeof(T).Name} with Partition Key {id.Partition} and Row Key{id.Row} from Azure Storage Table {table.Name} .", e);
                }
            }
            else if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"Delete failed; object of type {typeof(T).Name} with Partition Key {id.Partition} and Row Key{id.Row} does not exist in Azure Storage Table {table.Name}.");
            }
            else
            {
                throw new RepositoryException($"Delete failed; error code of {retrieveResult.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {id.Partition} and Row Key{id.Row} in Azure Storage Table {table.Name}.");
            }
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
                throw new NotFoundException($"Delete failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {table.Name}.");
            }

            // Create the Delete TableOperation.
            TableOperation operation = TableOperation.Delete(obj);

            try
            {
                // Execute the operation.
                TableResult result = table.ExecuteAsync(operation).GetAwaiter().GetResult();

                if (result.HttpStatusCode != (int)HttpStatusCode.OK)
                {
                    throw new RepositoryException($"Delete failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} in Azure Storage Table {table.Name}.");
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Delete failed; error deleting object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} from Azure Storage Table {table.Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.DeleteAsync(PK, CancellationToken)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The id parameter is null, or does not contain either a Partition Key or Row Key.</exception>
        public virtual async Task DeleteAsync(PK id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            TableResult retrieveResult = await GetRetrieveTableResultAsync(id.Partition, id.Row);

            if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                // Assign the result to be deleted.
                T deleteEntity = (T)retrieveResult.Result;

                // Create the Delete TableOperation.
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                TableResult deleteResult = await table.ExecuteAsync(deleteOperation);

                if (deleteResult.HttpStatusCode != (int)HttpStatusCode.OK)
                {
                    throw new RepositoryException($"Delete failed; error code of {deleteResult.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {id.Partition} and Row Key{id.Row} in Azure Storage Table {table.Name}.");
                }
            }
            else if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                throw new NotFoundException($"Delete failed; object of type {typeof(T).Name} with Partition Key {id.Partition} and Row Key{id.Row} does not exist in Azure Storage Table {table.Name}.");
            }
            else
            {
                throw new RepositoryException($"Delete failed; error code of {retrieveResult.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {id.Partition} and Row Key{id.Row} in Azure Storage Table {table.Name}.");
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
                throw new NotFoundException($"Delete failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {table.Name}.");
            }

            // Create the Delete TableOperation.
            TableOperation operation = TableOperation.Delete(obj);

            // Execute the operation.
            TableResult result = await table.ExecuteAsync(operation);

            if (result.HttpStatusCode != (int)HttpStatusCode.OK)
            {
                throw new RepositoryException($"Delete failed; error code of {result.HttpStatusCode} was returned for object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} in Azure Storage Table {table.Name}.");
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.DeleteBulk(IEnumerable{PK})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual void DeleteBulk(IEnumerable<PK> ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.DeleteBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual void DeleteBulk(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.DeleteBulkAsync(IEnumerable{PK}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task DeleteBulkAsync(IEnumerable<PK> ids, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.DeleteBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task DeleteBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Exists(PK)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The Partition and/or Row keys of the id parameter are null or empty.</exception>
        public virtual bool Exists(PK id)
        {
            return Exists(id.Partition, id.Row);
        }

        /// <summary>
        /// Check for existence of an entry.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>True if the entry exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The partitionKey and/or rowKey parameters are null or empty.</exception>
        /// <exception cref="RepositoryException">Error retrieving the object.</exception>
        private bool Exists(string partitionKey, string rowKey)
        {
            return (Retrieve(partitionKey, rowKey) != null);
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.ExistsAsync(PK, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            PK id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return (await ExistsAsync(id.Partition, id.Row));
        }

        /// <summary>
        /// Check for existence of an entry.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>True if the entry exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The partitionKey and/or rowKey parameters are null or empty.</exception>
        /// <exception cref="RepositoryException">Error retrieving the object.</exception>
        private async Task<bool> ExistsAsync(string partitionKey, string rowKey)
        {
            return ((await RetrieveAsync(partitionKey, rowKey)) != null);
        }

        /// <summary>
        /// Get the table result for a retrieve operation based upon partition and row keys.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>Storage table result.</returns>
        /// <exception cref="ArgumentNullException">The partitionKey and/or rowKey parameters are null or empty.</exception>
        /// <exception cref="RepositoryException">Error getting the storage table result.</exception>
        private TableResult GetRetrieveTableResult(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException("Unique identifier does not specify a Partition Key.", nameof(partitionKey));
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException("Unique identifier does not specify a Row Key.", nameof(rowKey));
            }

            TableResult result;

            // Create a retrieve operation that expects partition and row keys.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            // Execute the operation.
            try
            {
                result = table.ExecuteAsync(retrieveOperation).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error executing retrieve operation on object of type {typeof(T).Name} with Partition Key {partitionKey} and Row Key{rowKey} in Azure Storage Table {table.Name} .", e);
            }

            return result;
        }

        /// <summary>
        /// Get the table result for a retrieve operation based upon partition and row keys.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>Storage table result.</returns>
        /// <exception cref="ArgumentNullException">The partitionKey and/or rowKey parameters are null or empty.</exception>
        private async Task<TableResult> GetRetrieveTableResultAsync(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException("Unique identifier does not specify a Partition Key.", nameof(partitionKey));
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException("Unique identifier does not specify a Row Key.", nameof(rowKey));
            }

            // Create a retrieve operation that expects partition and row keys.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            // Execute the operation.
            TableResult result = await table.ExecuteAsync(retrieveOperation);

            return result;
        }

        /// <summary>
        /// The filter parameter is not used as it is not possible to query an Azure Storage Table using Linq.
        /// The sortCondition parameter has not been implemented and instead been defaulted to Timestamp (descending).
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// The current paging implementation is highly inefficient.
        /// <see cref="IRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">Not applicable.</exception>"
        public virtual IEnumerable<T> Retrieve(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            params Expression<Func<T, object>>[] includes)
        {
            IList<T> objs = new List<T>();

            try
            {
                // Construct the query operation for all objects.
                TableQuery<T> query = new TableQuery<T>();
                objs = table.ExecuteQueryAsync(query).GetAwaiter().GetResult();

                if (pagingContext?.PageSize > 0)
                {
                    objs = objs
                        .OrderByDescending(l => l.Timestamp)
                        .Skip((int)(pagingContext.PageIndex * pagingContext.PageSize))
                        .Take((int)pagingContext.PageSize)
                        .ToList();
                }
                else
                {
                    objs = objs.OrderByDescending(l => l.Timestamp).ToList();
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Retrieve failed; unable to retrieve objects of type {typeof(T).Name} from Azure Storage Table {table.Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// <see cref="IRepository{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row keys of the id parameter are null or empty.</exception>
        public virtual T Retrieve(PK id, params Expression<Func<T, object>>[] includes)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return Retrieve(id.Partition, id.Row);
        }

        /// <summary>
        /// Retrieve an entry by partition and row keys.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>Entry if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The partitionKey and/or rowKey parameters are null or empty.</exception>
        /// <exception cref="RepositoryException">Error retrieving the object.</exception>
        private T Retrieve(string partitionKey, string rowKey)
        {
            TableResult result = GetRetrieveTableResult(partitionKey, rowKey);

            T obj;

            if (result.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                obj = default(T);
            }
            else
            {
                throw new RepositoryException($"Error code of {result.HttpStatusCode} was returned retrieving object of type {typeof(T).Name} with Partition Key {partitionKey} and Row Key{rowKey} from Azure Storage Table {table.Name}.");
            }

            return obj;
        }

        /// <summary>
        /// The filter parameter is not used as it is not possible to query an Azure Storage Table using Linq.
        /// The sortCondition parameter has not been implemented and instead been defaulted to Timestamp (descending).
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// The current paging implementation is highly inefficient.
        /// <see cref="IRepository{T, PK}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">Not applicable.</exception>"
        public virtual async Task<IEnumerable<T>> RetrieveAsync(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<T, object>>[] includes)
        {
            IList<T> objs = new List<T>();

            // Construct the query operation for all objects.
            TableQuery<T> query = new TableQuery<T>();
            objs = await table.ExecuteQueryAsync(query);

            if (pagingContext?.PageSize > 0)
            {
                objs = objs
                    .OrderByDescending(l => l.Timestamp)
                    .Skip((int)(pagingContext.PageIndex * pagingContext.PageSize))
                    .Take((int)pagingContext.PageSize)
                    .ToList();
            }
            else
            {
                objs = objs.OrderByDescending(l => l.Timestamp).ToList();
            }

            return objs;
        }

        /// <summary>
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// <see cref="IRepository{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row values of the primary key (id) parameter are null or empty.</exception>
        public virtual async Task<T> RetrieveAsync(
            PK id,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<T, object>>[] includes)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await RetrieveAsync(id.Partition, id.Row);
        }

        /// <summary>
        /// Retrieve an entry by partition and row keys.
        /// </summary>
        /// <param name="partitionKey">Partition key.</param>
        /// <param name="rowKey">Row key.</param>
        /// <returns>Entry if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The partitionKey and/or rowKey parameters are null or empty.</exception>
        /// <exception cref="RepositoryException">Error retrieving the object.</exception>
        private async Task<T> RetrieveAsync(string partitionKey, string rowKey)
        {
            TableResult result = await GetRetrieveTableResultAsync(partitionKey, rowKey);

            T obj;

            if (result.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                obj = default(T);
            }
            else
            {
                throw new RepositoryException($"Error code of {result.HttpStatusCode} was returned retrieving object of type {typeof(T).Name} with Partition Key {partitionKey} and Row Key{rowKey} from Azure Storage Table {table.Name}.");
            }

            return obj;
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
                throw new NotFoundException($"Update failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {table.Name}.");
            }

            try
            {
                // Create the InsertOrReplace TableOperation.
                TableOperation insertOrReplaceOperation = TableOperation.Replace(obj);

                // Execute the operation.
                table.ExecuteAsync(insertOrReplaceOperation).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Update failed; error updating object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} from Azure Storage Table {table.Name}.", e);
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
                throw new NotFoundException($"Update failed; object of type {typeof(T).Name} with Partition Key {obj.PartitionKey} and Row Key{obj.RowKey} does not exist in Azure Storage Table {table.Name}.");
            }

            // Create the InsertOrReplace TableOperation.
            TableOperation insertOrReplaceOperation = TableOperation.Replace(obj);

            // Execute the operation.
            await table.ExecuteAsync(insertOrReplaceOperation);
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.UpdateBulk(IEnumerable{T})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual void UpdateBulk(IEnumerable<T> objs)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IBulkRepository{T, PK}.UpdateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task UpdateBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}