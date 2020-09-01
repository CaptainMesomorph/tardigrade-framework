using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Base repository interface of CRUD (create, retrieve, update and delete) operations on objects of a particular
    /// type.
    /// <a href="https://cpratt.co/truly-generic-repository/">A Truly Generic Repository, Part 1</a>
    /// </summary>
    /// <typeparam name="T">Object type associated with the repository operations.</typeparam>
    /// <typeparam name="Pk">Unique identifier type for the object type.</typeparam>
    public interface IRepository<T, in Pk> : IReadOnlyRepository<T, Pk>, IBulkRepository<T>
    {
        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="item">Instance to create.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="Exceptions.AlreadyExistsException">Object already exists.</exception>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to create contains invalid values.</exception>
        T Create(T item);

        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="item">Instance to create.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="Exceptions.AlreadyExistsException">Object already exists.</exception>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to create contains invalid values.</exception>
        Task<T> CreateAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="item">Instance to delete.</param>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        void Delete(T item);

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="item">Instance to delete.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation result.</returns>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        Task DeleteAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="item">Instance to update.</param>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error updating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to update contains invalid values.</exception>
        void Update(T item);

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="item">Instance to update.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation result.</returns>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to update does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error updating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to update contains invalid values.</exception>
        Task UpdateAsync(T item, CancellationToken cancellationToken = default);
    }
}