using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Base repository interface of bulk operations on objects of a particular type. Bulk operations process a
    /// collection of objects at once, and success or failure applies to the collection as a whole.
    /// </summary>
    /// <typeparam name="TEntity">Object type associated with the repository operations.</typeparam>
    public interface IBulkRepository<TEntity>
    {
        /// <summary>
        /// Create multiple instances of the object type.
        /// </summary>
        /// <param name="items">Instances to create.</param>
        /// <returns>Instances created (including allocated unique identifiers).</returns>
        /// <exception cref="ArgumentNullException">The items parameter is null or empty.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the objects.</exception>
        IEnumerable<TEntity> CreateBulk(IEnumerable<TEntity> items);

        /// <summary>
        /// Create multiple instances of the object type.
        /// </summary>
        /// <param name="items">Instances to create.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Instances created (including allocated unique identifiers).</returns>
        /// <exception cref="ArgumentNullException">The items parameter is null or empty.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the objects.</exception>
        Task<IEnumerable<TEntity>> CreateBulkAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete multiple instances.
        /// </summary>
        /// <param name="items">Instances to delete.</param>
        /// <exception cref="ArgumentNullException">The items parameter is null or empty.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the objects.</exception>
        void DeleteBulk(IEnumerable<TEntity> items);

        /// <summary>
        /// Delete multiple instances.
        /// </summary>
        /// <param name="items">Instances to delete.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation result.</returns>
        /// <exception cref="ArgumentNullException">The items parameter is null or empty.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the objects.</exception>
        Task DeleteBulkAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update multiple instances.
        /// </summary>
        /// <param name="items">Instances to update.</param>
        /// <exception cref="ArgumentNullException">The items parameter is null or empty.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error updating the objects.</exception>
        void UpdateBulk(IEnumerable<TEntity> items);

        /// <summary>
        /// Update multiple instances.
        /// </summary>
        /// <param name="items">Instances to update.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation result.</returns>
        /// <exception cref="ArgumentNullException">The items parameter is null or empty.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error updating the objects.</exception>
        Task UpdateBulkAsync(IEnumerable<TEntity> items, CancellationToken cancellationToken = default);
    }
}