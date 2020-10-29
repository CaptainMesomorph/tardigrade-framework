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

namespace Tardigrade.Framework.AzureStorage.Tables
{
    /// <summary>
    /// Repository layer that is based on Azure Storage Tables.
    /// <see cref="IReadOnlyRepository{TEntity, TKey}"/>
    /// </summary>
    public class ReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
        where TEntity : ITableEntity, new()
        where TKey : ITableKey
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
        public ReadOnlyRepository(string connectionString, string tableName)
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
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Count(Expression{Func{TEntity, bool}})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.CountAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">To be implemented.</exception>
        public virtual Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Exists(TKey)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The Partition and/or Row keys of the id parameter are null or empty.</exception>
        public virtual bool Exists(TKey id)
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
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.ExistsAsync(TKey, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            TKey id,
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
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Retrieve(Expression{Func{TEntity, bool}}, PagingContext, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">Not applicable.</exception>"
        public virtual IEnumerable<TEntity> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IList<TEntity> items;

            try
            {
                // Construct the query operation for all objects.
                var query = new TableQuery<TEntity>();
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
                    $"Retrieve failed; unable to retrieve objects of type {typeof(TEntity).Name} from Azure Storage Table {Table.Name}.",
                    e);
            }

            return items;
        }

        /// <summary>
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Retrieve(TKey, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row keys of the id parameter are null or empty.</exception>
        public virtual TEntity Retrieve(TKey id, params Expression<Func<TEntity, object>>[] includes)
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
        private TEntity Retrieve(string partitionKey, string rowKey)
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
            TableOperation retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            // Execute the operation.
            try
            {
                result = AsyncHelper.RunSync(() => Table.ExecuteAsync(retrieveOperation));
            }
            catch (Exception e)
            {
                throw new RepositoryException(
                    $"Error executing retrieve operation on object of type {typeof(TEntity).Name} with Partition Key {partitionKey} and Row Key{rowKey} in Azure Storage Table {Table.Name} .",
                    e);
            }

            TEntity obj;

            if (result.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                obj = (TEntity)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                obj = default;
            }
            else
            {
                throw new RepositoryException(
                    $"Error code of {result.HttpStatusCode} was returned retrieving object of type {typeof(TEntity).Name} with Partition Key {partitionKey} and Row Key{rowKey} from Azure Storage Table {Table.Name}.");
            }

            return obj;
        }

        /// <summary>
        /// The filter parameter is not used as it is not possible to query an Azure Storage Table using Linq.
        /// The sortCondition parameter has not been implemented and instead been defaulted to Timestamp (descending).
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// The current paging implementation is highly inefficient.
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.RetrieveAsync(Expression{Func{TEntity, bool}}, PagingContext, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}, CancellationToken, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">Not applicable.</exception>"
        public virtual async Task<IEnumerable<TEntity>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            // Construct the query operation for all objects.
            var query = new TableQuery<TEntity>();
            IList<TEntity> items = await Table.ExecuteQueryAsync(query, cancellationToken);

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
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Retrieve(TKey, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row values of the primary key (id) parameter are null or empty.</exception>
        public virtual async Task<TEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
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
        private async Task<TEntity> RetrieveAsync(string partitionKey, string rowKey)
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
            TableOperation retrieveOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);

            // Execute the operation.
            TableResult result = await Table.ExecuteAsync(retrieveOperation);

            TEntity obj;

            if (result.HttpStatusCode == (int)HttpStatusCode.OK)
            {
                obj = (TEntity)result.Result;
            }
            else if (result.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                obj = default;
            }
            else
            {
                throw new RepositoryException(
                    $"Error code of {result.HttpStatusCode} was returned retrieving object of type {typeof(TEntity).Name} with Partition Key {partitionKey} and Row Key{rowKey} from Azure Storage Table {Table.Name}.");
            }

            return obj;
        }
    }
}