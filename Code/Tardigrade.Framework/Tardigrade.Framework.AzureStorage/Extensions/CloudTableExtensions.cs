using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tardigrade.Framework.AzureStorage.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the CloudTable type.
    /// </summary>
    public static class CloudTableExtensions
    {
        /// <summary>
        /// Extension for simplifying the execution of an asynchronous query.
        /// </summary>
        /// <typeparam name="T">Type of object associated with the query.</typeparam>
        /// <param name="table">Storage table.</param>
        /// <param name="query">Query to execute.</param>
        /// <param name="cancellationToken">Token for handling operation cancellation.</param>
        /// <returns>Result of the query.</returns>
        /// <exception cref="ArgumentNullException">The query parameter is null.</exception>
        public static async Task<IList<T>> ExecuteQueryAsync<T>(
            this CloudTable table,
            TableQuery<T> query,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : ITableEntity, new()
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            TableQuery<T> runningQuery = new TableQuery<T>()
            {
                FilterString = query.FilterString,
                SelectColumns = query.SelectColumns,
                TakeCount = query.TakeCount
            };

            List<T> items = new List<T>();
            TableContinuationToken token = null;

            do
            {
                runningQuery.TakeCount = query.TakeCount - items.Count;
                TableQuerySegment<T> segment = await table.ExecuteQuerySegmentedAsync(runningQuery, token);
                token = segment.ContinuationToken;
                items.AddRange(segment);
            } while (
                token != null &&
                !cancellationToken.IsCancellationRequested &&
                (query.TakeCount == null || items.Count < query.TakeCount.Value));

            return items;
        }
    }
}