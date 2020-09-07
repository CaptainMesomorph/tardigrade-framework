using Microsoft.Azure.Cosmos.Table;
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
using Tardigrade.Framework.Helpers;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.AzureStorage
{
    /// <summary>
    /// Repository layer that is based on Azure Storage Tables.
    /// <see cref="IRepository{T, Pk}"/>
    /// </summary>
    public class StorageTableReadOnlyRepository<T, Pk> : IReadOnlyRepository<T, Pk>
        where T : ITableEntity, new()
        where Pk : ITableKey
    {
        /// <summary>
        /// Azure Storage Table containing the object types.
        /// </summary>
        protected CloudTable Table { get; }

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
        public StorageTableReadOnlyRepository(string connectionString, string tableName)
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
            Table = tableClient.GetTableReference(tableName);

            // Create the table if it doesn't exist.
            try
            {
                AsyncHelper.RunSync(() => Table.CreateIfNotExistsAsync());
            }
            catch (StorageException e) when (e.InnerException is HttpRequestException)
            {
                throw new FormatException(
                    $"A Storage Account endpoint in the following connection string is invalid:\n{connectionString}",
                    e);
            }
            catch (StorageException e)
            {
                throw new FormatException(
                    $"Storage Account name in the following connection string is not recognised:\n{connectionString}",
                    e);
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Failed to create Azure Storage Table {Table.Name} using the following connection string.\n{connectionString}",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, Pk}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, Pk}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task<int> CountAsync(
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, Pk}.Exists(Pk)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The Partition and/or Row keys of the id parameter are null or empty.</exception>
        public virtual bool Exists(Pk id)
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
        protected bool Exists(string partitionKey, string rowKey)
        {
            return (Retrieve(partitionKey, rowKey) != null);
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, Pk}.ExistsAsync(Pk, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            Pk id,
            CancellationToken cancellationToken = default)
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
        protected async Task<bool> ExistsAsync(string partitionKey, string rowKey)
        {
            return ((await RetrieveAsync(partitionKey, rowKey)) != null);
        }

        /// <summary>
        /// The filter parameter is not used as it is not possible to query an Azure Storage Table using Linq.
        /// The sortCondition parameter has not been implemented and instead been defaulted to Timestamp (descending).
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// The current paging implementation is highly inefficient.
        /// <see cref="IReadOnlyRepository{T, Pk}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">Not applicable.</exception>"
        public virtual IEnumerable<T> Retrieve(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            params Expression<Func<T, object>>[] includes)
        {
            IList<T> items;

            try
            {
                // Construct the query operation for all objects.
                var query = new TableQuery<T>();
                items = AsyncHelper.RunSync(() => Table.ExecuteQueryAsync(query));

                if (pagingContext?.PageSize > 0)
                {
                    items = items
                        .OrderByDescending(l => l.Timestamp)
                        .Skip((int)(pagingContext.PageIndex * pagingContext.PageSize))
                        .Take((int)pagingContext.PageSize)
                        .ToList();
                }
                else
                {
                    items = items.OrderByDescending(l => l.Timestamp).ToList();
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Retrieve failed; unable to retrieve objects of type {typeof(T).Name} from Azure Storage Table {Table.Name}.",
                    e);
            }

            return items;
        }

        /// <summary>
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// <see cref="IReadOnlyRepository{T, Pk}.Retrieve(Pk, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row keys of the id parameter are null or empty.</exception>
        public virtual T Retrieve(Pk id, params Expression<Func<T, object>>[] includes)
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
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException(nameof(partitionKey), "Unique identifier does not specify a Partition Key.");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException(nameof(rowKey), "Unique identifier does not specify a Row Key.");
            }

            TableResult result;

            // Create a retrieve operation that expects partition and row keys.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            // Execute the operation.
            try
            {
                result = AsyncHelper.RunSync(() => Table.ExecuteAsync(retrieveOperation));
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Error executing retrieve operation on object of type {typeof(T).Name} with Partition Key {partitionKey} and Row Key{rowKey} in Azure Storage Table {Table.Name} .",
                    e);
            }

            T obj;

            if (result.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                obj = default;
            }
            else
            {
                throw new RepositoryException(
                    $"Error code of {result.HttpStatusCode} was returned retrieving object of type {typeof(T).Name} with Partition Key {partitionKey} and Row Key{rowKey} from Azure Storage Table {Table.Name}.");
            }

            return obj;
        }

        /// <summary>
        /// The filter parameter is not used as it is not possible to query an Azure Storage Table using Linq.
        /// The sortCondition parameter has not been implemented and instead been defaulted to Timestamp (descending).
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// The current paging implementation is highly inefficient.
        /// <see cref="IReadOnlyRepository{T, Pk}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">Not applicable.</exception>"
        public virtual async Task<IEnumerable<T>> RetrieveAsync(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            // Construct the query operation for all objects.
            var query = new TableQuery<T>();
            IList<T> items = await Table.ExecuteQueryAsync(query, cancellationToken);

            if (pagingContext?.PageSize > 0)
            {
                items = items
                    .OrderByDescending(l => l.Timestamp)
                    .Skip((int)(pagingContext.PageIndex * pagingContext.PageSize))
                    .Take((int)pagingContext.PageSize)
                    .ToList();
            }
            else
            {
                items = items.OrderByDescending(l => l.Timestamp).ToList();
            }

            return items;
        }

        /// <summary>
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// <see cref="IReadOnlyRepository{T, Pk}.Retrieve(Pk, Expression{Func{T, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row values of the primary key (id) parameter are null or empty.</exception>
        public virtual async Task<T> RetrieveAsync(
            Pk id,
            CancellationToken cancellationToken = default,
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
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException(nameof(partitionKey), "Unique identifier does not specify a Partition Key.");
            }

            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException(nameof(rowKey), "Unique identifier does not specify a Row Key.");
            }

            // Create a retrieve operation that expects partition and row keys.
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            // Execute the operation.
            TableResult result = await Table.ExecuteAsync(retrieveOperation);

            T obj;

            if (result.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                obj = (T)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                obj = default;
            }
            else
            {
                throw new RepositoryException(
                    $"Error code of {result.HttpStatusCode} was returned retrieving object of type {typeof(T).Name} with Partition Key {partitionKey} and Row Key{rowKey} from Azure Storage Table {Table.Name}.");
            }

            return obj;
        }
    }
}