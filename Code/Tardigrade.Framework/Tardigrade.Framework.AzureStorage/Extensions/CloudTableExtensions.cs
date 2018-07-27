﻿using Microsoft.WindowsAzure.Storage.Table;
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
        public static async Task<IList<T>> ExecuteQueryAsync<T>(
            this CloudTable table,
            TableQuery<T> query,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : ITableEntity, new()
        {
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