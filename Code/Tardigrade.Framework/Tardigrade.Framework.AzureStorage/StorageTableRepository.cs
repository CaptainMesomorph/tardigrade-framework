using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Tardigrade.Framework.AzureStorage.Extensions;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.AzureStorage
{
    /// <summary>
    /// Repository layer that is based on Azure Storage Tables.
    /// <see cref="IRepository{T, PK}"/>
    /// </summary>
    public class StorageTableRepository<T, PK> : IRepository<T, PK> where T : ITableEntity, new()
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
                throw new ArgumentNullException("tableName");
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
        /// <see cref="IRepository{T, PK}.Create(T, IUnitOfWork)"/>
        /// </summary>
        public virtual T Create(T obj, IUnitOfWork unitOfWork = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
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
            catch (Exception e)
            {
                throw new RepositoryException($"Error creating an object of type {typeof(T).Name} in Azure Storage Table {table.Name}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Delete(PK, IUnitOfWork)"/>
        /// </summary>
        public virtual void Delete(PK id, IUnitOfWork unitOfWork = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                // Construct the query operation for all objects where RowKey=id.
                TableQuery<T> query = new TableQuery<T>()
                    .Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
                T obj = table.ExecuteQueryAsync(query).GetAwaiter().GetResult().Single();

                // Create the Delete TableOperation.
                if (obj == null)
                {
                    if (log.IsWarnEnabled) log.Warn($"Object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of {id.ToString()} cannot be deleted as it does not exist.");
                }
                else
                {
                    TableOperation deleteOperation = TableOperation.Delete(obj);

                    // Execute the operation.
                    table.ExecuteAsync(deleteOperation).GetAwaiter().GetResult();
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of {id.ToString()}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Delete(T, IUnitOfWork)"/>
        /// </summary>
        public virtual void Delete(T obj, IUnitOfWork unitOfWork = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            try
            {
                // Create a retrieve operation that expects partition and row keys.
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(obj.PartitionKey, obj.RowKey);

                // Execute the operation.
                TableResult retrieveResult = table.ExecuteAsync(retrieveOperation).GetAwaiter().GetResult();

                if (retrieveResult.HttpStatusCode == (int)HttpStatusCode.OK)
                {
                    T deleteObj = (T)retrieveResult.Result;

                    // Create the Delete TableOperation.
                    if (deleteObj == null)
                    {
                        if (log.IsWarnEnabled) log.Warn($"Object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of {obj.RowKey} cannot be deleted as it does not exist.");
                    }
                    else
                    {
                        TableOperation deleteOperation = TableOperation.Delete(deleteObj);

                        // Execute the operation.
                        table.ExecuteAsync(deleteOperation).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of {obj.RowKey}. HTTP status code of {retrieveResult.HttpStatusCode} was returned.");
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of {obj.RowKey}.", e);
            }
        }

        /// <summary>
        /// The predicate parameter is not used as it is not possible to query an Azure Storage Table using Linq.
        /// <see cref="IRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, int?, int?, string[])"/>
        /// </summary>
        public virtual IList<T> Retrieve(Expression<Func<T, bool>> predicate = null, int? pageIndex = null, int? pageSize = null, string[] includes = null)
        {
            IList<T> objs = new List<T>();

            try
            {
                // Construct the query operation for all objects.
                TableQuery<T> query = new TableQuery<T>();
                objs = table.ExecuteQueryAsync(query).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error retrieving objects of type {typeof(T).Name} from Azure Storage Table {table.Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Retrieve(PK, string[])"/>
        /// </summary>
        public virtual T Retrieve(PK id, string[] includes = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            T obj = default(T);

            try
            {
                // Construct the query operation for all objects where RowKey=id.
                TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id.ToString()));
                IEnumerable<T> objs = table.ExecuteQueryAsync(query).GetAwaiter().GetResult();

                if (objs.Count() > 0)
                {
                    obj = objs.Single();
                }
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error retrieving an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of {id.ToString()}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Update(T, IUnitOfWork)"/>
        /// </summary>
        public virtual void Update(T obj, IUnitOfWork unitOfWork = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            try
            {
                // Create the InsertOrReplace TableOperation.
                TableOperation insertOrReplaceOperation = TableOperation.InsertOrReplace(obj);

                // Execute the operation.
                table.ExecuteAsync(insertOrReplaceOperation).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error updating an object of type {typeof(T).Name} from Azure Storage Table {table.Name} with unique identifier of {obj.RowKey}.", e);
            }
        }
    }
}