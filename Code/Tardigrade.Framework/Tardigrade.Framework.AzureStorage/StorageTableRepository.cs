using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Tardigrade.Framework.AzureStorage.Extensions;
using Tardigrade.Framework.AzureStorage.Models;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.AzureStorage
{
    /// <summary>
    /// Repository layer that is based on Azure Storage Tables.
    /// <see cref="IRepository{T, PK}"/>
    /// </summary>
    public class StorageTableRepository<T, PK> : IRepository<T, PK> where T : ITableEntity, new() where PK : ITableKey
    {
        private static readonly slf4net.ILogger log = slf4net.LoggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string storageConnectionString;
        private readonly CloudTable table;

        /// <summary>
        /// Create an instance of this repository.
        /// </summary>
        /// <param name="storageConnectionString">Storage Table connection string.</param>
        /// <param name="tableName">Name of the Storage Table.</param>
        /// <exception cref="ArgumentException">storageConnectionString cannot be parsed.</exception>
        /// <exception cref="ArgumentNullException">storageConnectionString or tableName is null or empty.</exception>
        /// <exception cref="FormatException">storageConnectionString is not a valid connection string.</exception>
        public StorageTableRepository(string storageConnectionString, string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            this.storageConnectionString = storageConnectionString;

            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            table = tableClient.GetTableReference(tableName);

            // Create the table if it doesn't exist.
            table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Not implemented.
        /// <see cref="IRepository{T, PK}.Count()"/>
        /// </summary>
        public virtual int Count()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ValidationException currently not supported.
        /// <see cref="ICrudRepository{T, PK}.Create(T)"/>
        /// </summary>
        public virtual T Create(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                // Create the TableOperation that inserts the object.
                TableOperation insertOperation = TableOperation.Insert(obj, true);

                // Execute the insert operation.
                TableResult result = table.ExecuteAsync(insertOperation).GetAwaiter().GetResult();

                if (result.HttpStatusCode == (int)HttpStatusCode.Created)
                {
                    obj = (T)result.Result;
                }
                else
                {
                    throw new RepositoryException($"Error creating an object of type {typeof(T).Name} in Azure Storage Table {table.Name}. HTTP status code of {result.HttpStatusCode} was returned.");
                }
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error creating an object of type {typeof(T).Name} in Azure Storage Table {table.Name}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="ICrudRepository{T, PK}.Delete(PK)"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row values of the primary key (id) parameter are either null or empty.</exception>
        public virtual void Delete(PK id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(id.Partition))
            {
                throw new ArgumentException("The primary key does not specify a Partition value.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(id.Row))
            {
                throw new ArgumentException("The primary key does not specify a Row value.", nameof(id));
            }

            try
            {
                // Create a retrieve operation that expects partition and row keys.
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(id.Partition, id.Row);

                // Execute the operation.
                TableResult retrievedResult = table.ExecuteAsync(retrieveOperation).GetAwaiter().GetResult();

                if (retrievedResult.HttpStatusCode == (int)HttpStatusCode.OK)
                {
                    // Assign the result to be deleted.
                    T deleteEntity = (T)retrievedResult.Result;

                    // Create the Delete TableOperation.
                    TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                    // Execute the operation.
                    table.ExecuteAsync(deleteOperation).GetAwaiter().GetResult();
                }
                else if (retrievedResult.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    if (log.IsWarnEnabled) log.Warn($"Object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of [Partition={id.Partition.ToString()},Row={id.Row.ToString()}] cannot be deleted as it does not exist.");
                }
                else
                {
                    throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of [Partition={id.Partition.ToString()},Row={id.Row.ToString()}]. HTTP status code of {retrievedResult.HttpStatusCode} was returned.");
                }
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of [Partition={id.Partition.ToString()},Row={id.Row.ToString()}].", e);
            }
        }

        /// <summary>
        /// <see cref="ICrudRepository{T, PK}.Delete(T)"/>
        /// </summary>
        public virtual void Delete(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                TableOperation deleteOperation = TableOperation.Delete(obj);

                // Execute the operation.
                table.ExecuteAsync(deleteOperation).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of [Partition={obj.PartitionKey?.ToString()},Row={obj.RowKey?.ToString()}].", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Exists(PK)"/>
        /// </summary>
        public virtual bool Exists(PK id)
        {
            return Retrieve(id) != null;
        }

        /// <summary>
        /// The predicate parameter is not used as it is not possible to query an Azure Storage Table using Linq.
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// <see cref="ICrudRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, int?, int?, string[])"/>
        /// </summary>
        public virtual IList<T> Retrieve(Expression<Func<T, bool>> predicate = null, int? pageIndex = null, int? pageSize = null, string[] includes = null)
        {
            IList<T> objs = new List<T>();

            try
            {
                // Construct the query operation for all objects.
                TableQuery<T> query = new TableQuery<T>();
                objs = table.ExecuteQueryAsync(query).GetAwaiter().GetResult();

                if ((pageIndex.HasValue && pageIndex.Value >= 0) && (pageSize.HasValue && pageSize.Value > 0))
                {
                    objs = objs.OrderByDescending(l => l.Timestamp).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList();
                }
                else
                {
                    objs = objs.OrderByDescending(l => l.Timestamp).ToList();
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error retrieving objects of type {typeof(T).Name} from Azure Storage Table {table.Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// The includes parameter is not applicable to Azure Storage Tables.
        /// <see cref="ICrudRepository{T, PK}.Retrieve(PK, string[])"/>
        /// </summary>
        /// <exception cref="ArgumentException">The Partition and/or Row values of the primary key (id) parameter are either null or empty.</exception>
        public virtual T Retrieve(PK id, string[] includes = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(id.Partition))
            {
                throw new ArgumentException("The primary key does not specify a Partition value.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(id.Row))
            {
                throw new ArgumentException("The primary key does not specify a Row value.", nameof(id));
            }

            T obj;

            try
            {
                // Create a retrieve operation that expects partition and row keys.
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(id.Partition, id.Row);

                // Execute the operation.
                TableResult retrievedResult = table.ExecuteAsync(retrieveOperation).GetAwaiter().GetResult();

                if (retrievedResult.HttpStatusCode == (int)HttpStatusCode.OK)
                {
                    obj = (T)retrievedResult.Result;
                }
                else if (retrievedResult.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    obj = default(T);
                }
                else
                {
                    throw new RepositoryException($"Error retrieving an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of [Partition={id.Partition.ToString()},Row={id.Row.ToString()}]. HTTP status code of {retrievedResult.HttpStatusCode} was returned.");
                }
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error retrieving an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of [Partition={id.Partition.ToString()},Row={id.Row.ToString()}].", e);
            }

            return obj;
        }

        /// <summary>
        /// ValidationException currently not supported.
        /// <see cref="ICrudRepository{T, PK}.Update(T)"/>
        /// </summary>
        public virtual void Update(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
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
                throw new RepositoryException($"Error updating an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of [Partition={obj.PartitionKey?.ToString()},Row={obj.RowKey?.ToString()}].", e);
            }
        }
    }
}