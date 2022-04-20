using Audit.Core;
using Azure.Storage.Queues.Models;
using System;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;

#if NET
using System.Text.Json;
#else
using Newtonsoft.Json;
#endif

namespace Tardigrade.Framework.AuditNET.AzureStorageQueue.DataProviders
{
    /// <inheritdoc />
    public class AzureQueueDataProvider : AuditDataProvider
    {
        private readonly QueueClient _queueClient;

        /// <summary>
        /// Create an instance of this Data Provider.
        /// </summary>
        /// <param name="connectionString">Azure Storage Account connection string.</param>
        /// <param name="queueName">Azure Storage Queue name.</param>
        /// <exception cref="ArgumentNullException">A parameter is null or empty.</exception>
        /// <exception cref="QueueException">Error creating the Azure Storage Queue.</exception>
        public AzureQueueDataProvider(string connectionString, string queueName)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

            // Create the queue client.
            _queueClient = new QueueClient(connectionString, queueName);

            try
            {
                // Create the queue if it doesn't already exist.
                _queueClient.CreateIfNotExists();
            }
            catch (RequestFailedException e)
            {
                throw new QueueException($"Error creating Azure Storage Queue {queueName}: {e.Message}.", e);
            }
        }

        /// <summary>
        /// <see cref="AuditDataProvider.InsertEvent(AuditEvent)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">Parameter is null or empty.</exception>
        public override object InsertEvent(AuditEvent auditEvent)
        {
            if (auditEvent == null) throw new ArgumentNullException(nameof(auditEvent));

            // Create a message and add it to the queue.
#if NET
            string message = JsonSerializer.Serialize(auditEvent, Configuration.JsonSettings);
#else
            string message = JsonConvert.SerializeObject(auditEvent, Configuration.JsonSettings);
#endif
            SendReceipt receipt = _queueClient.SendMessage(message.ToBase64());

            return receipt;
        }

        /// <summary>
        /// <see cref="AuditDataProvider.InsertEventAsync(AuditEvent)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">Parameter is null or empty.</exception>
        public override async Task<object> InsertEventAsync(AuditEvent auditEvent)
        {
            if (auditEvent == null) throw new ArgumentNullException(nameof(auditEvent));

            // Create a message and add it to the queue.
#if NET
            string message = JsonSerializer.Serialize(auditEvent, Configuration.JsonSettings);
#else
            string message = JsonConvert.SerializeObject(auditEvent, Configuration.JsonSettings);
#endif
            SendReceipt receipt = await _queueClient.SendMessageAsync(message.ToBase64());

            return receipt;
        }
    }
}