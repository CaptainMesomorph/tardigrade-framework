using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Models.Persistence;

namespace Tardigrade.Framework.Services
{
    /// <summary>
    /// Base service interface for objects of a particular type.
    /// </summary>
    /// <typeparam name="T">Object type associated with the service operations.</typeparam>
    /// <typeparam name="Pk">Unique identifier type for the object type.</typeparam>
    public interface IObjectService<T, in Pk>
    {
        /// <summary>
        /// Calculate the number of objects of this type.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <returns>Number of objects of this type.</returns>
        /// <exception cref="Exceptions.ServiceException">Error calculating the number of objects.</exception>
        int Count(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Calculate the number of objects of this type.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Number of objects of this type.</returns>
        /// <exception cref="Exceptions.ServiceException">Error calculating the number of objects.</exception>
        Task<int> CountAsync(
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create multiple instances of the object type.
        /// </summary>
        /// <param name="items">Instances to create.</param>
        /// <returns>Instances created (including allocated unique identifiers).</returns>
        /// <exception cref="ArgumentNullException">The items parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error creating the objects.</exception>
        IEnumerable<T> Create(IEnumerable<T> items);

        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="item">Instance to create.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error creating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to create contains invalid values.</exception>
        T Create(T item);

        /// <summary>
        /// Create multiple instances of the object type.
        /// </summary>
        /// <param name="items">Instances to create.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Instances created (including allocated unique identifiers).</returns>
        /// <exception cref="ArgumentNullException">The items parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error creating the objects.</exception>
        Task<IEnumerable<T>> CreateAsync(
            IEnumerable<T> items,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="item">Instance to create.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error creating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to create contains invalid values.</exception>
        Task<T> CreateAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="item">Instance to delete.</param>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.ServiceException">Error deleting the object.</exception>
        void Delete(T item);

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="item">Instance to delete.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation result.</returns>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.ServiceException">Error deleting the object.</exception>
        Task DeleteAsync(T item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check for existence of an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error checking for existence of an object.</exception>
        bool Exists(Pk id);

        /// <summary>
        /// Check for existence of an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error checking for existence of an object.</exception>
        Task<bool> ExistsAsync(Pk id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve all instances of the object type. If paging parameters are provided, a range of objects is
        /// returned.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="ArgumentException">A sortCondition is required if pagingContext is provided.</exception>
        /// <exception cref="Exceptions.ServiceException">Error retrieving the objects.</exception>
        IEnumerable<T> Retrieve(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Retrieve an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Instance if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error retrieving the object.</exception>
        T Retrieve(Pk id, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Retrieve all instances of the object type. If paging parameters are provided, a range of objects is
        /// returned.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="ArgumentException">A sortCondition is required if pagingContext is provided.</exception>
        /// <exception cref="Exceptions.ServiceException">Error retrieving the objects.</exception>
        Task<IEnumerable<T>> RetrieveAsync(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Retrieve an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Instance if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error retrieving the object.</exception>
        Task<T> RetrieveAsync(
            Pk id,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="item">Instance to update.</param>
        /// <exception cref="ArgumentNullException">The item parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to update does not exist.</exception>
        /// <exception cref="Exceptions.ServiceException">Error updating the object.</exception>
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
        /// <exception cref="Exceptions.ServiceException">Error updating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to update contains invalid values.</exception>
        Task UpdateAsync(T item, CancellationToken cancellationToken = default);
    }
}