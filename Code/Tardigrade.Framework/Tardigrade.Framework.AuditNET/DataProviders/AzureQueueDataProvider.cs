using Audit.Core;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Helpers;

namespace Tardigrade.Framework.AuditNET.DataProviders
{
    /// <summary>
    /// <see cref="AuditDataProvider"/>
    /// </summary>
    public class AzureQueueDataProvider : AuditDataProvider
    {
        private readonly CloudQueue queue;

        /// <summary>
        /// Create an instance of this Data Provider.
        /// </summary>
        /// <param name="connectionString">Azure Storage Account connection string.</param>
        /// <param name="queueName">Azure Storage Queue name.</param>
        /// <exception cref="ArgumentException">Azure Storage Account connection string cannot be parsed.</exception>
        /// <exception cref="ArgumentNullException">A parameter is null or empty.</exception>
        /// <exception cref="FormatException">Azure Storage Account connection string is not a valid connection string.</exception>
        /// <exception cref="QueueException">Error creating the Azure Storage Queue.</exception>
        public AzureQueueDataProvider(string connectionString, string queueName)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(queueName))
            {
                throw new ArgumentNullException(nameof(queueName));
            }

            // Retrieve storage account from connection string.
            // Parse() may throw ArgumentException or FormatException.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            queue = queueClient.GetQueueReference(queueName);

            try
            {
                // Create the queue if it doesn't already exist.
                // CreateIfNotExistsAsync() may throw StorageException.
                AsyncHelper.RunSync(() => queue.CreateIfNotExistsAsync());
            }
            catch (StorageException e)
            {
                throw new QueueException($"Error creating Azure Storage Queue {queueName}: {e.GetBaseException().Message}.", e);
            }
        }

        /// <summary>
        /// <see cref="AuditDataProvider.InsertEvent(AuditEvent)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">Parameter is null or empty.</exception>
        public override object InsertEvent(AuditEvent auditEvent)
        {
            return AsyncHelper.RunSync(() => InsertEventAsync(auditEvent));
        }

        /// <summary>
        /// <see cref="AuditDataProvider.InsertEventAsync(AuditEvent)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">Parameter is null or empty.</exception>
        public override async Task<object> InsertEventAsync(AuditEvent auditEvent)
        {
            if (auditEvent == null)
            {
                throw new ArgumentNullException(nameof(auditEvent));
            }

            // Create a message and add it to the queue.
            CloudQueueMessage message =
                new CloudQueueMessage(JsonConvert.SerializeObject(auditEvent, Configuration.JsonSettings));
            await queue.AddMessageAsync(message);

            return message.Id;
        }
    }
}