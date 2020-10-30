using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Models.Persistence;

namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Base repository interface of CRUD (create, retrieve, update and delete) operations on objects of a particular
    /// type.
    /// <a href="https://cpratt.co/truly-generic-repository/">A Truly Generic Repository, Part 1</a>
    /// </summary>
    /// <typeparam name="TEntity">Object type associated with the repository operations.</typeparam>
    /// <typeparam name="TKey">Unique identifier type for the object type.</typeparam>
    public interface IReadOnlyRepository<TEntity, in TKey>
    {
        /// <summary>
        /// Calculate the number of objects in the repository.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <returns>Number of objects in the repository.</returns>
        /// <exception cref="Exceptions.RepositoryException">Error calculating the number of objects.</exception>
        int Count(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Calculate the number of objects in the repository.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Number of objects in the repository.</returns>
        /// <exception cref="Exceptions.RepositoryException">Error calculating the number of objects.</exception>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check for existence of an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error checking for existence of an object.</exception>
        bool Exists(TKey id);

        /// <summary>
        /// Check for existence of an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error checking for existence of an object.</exception>
        Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve all instances of the object type. If paging parameters are provided, a range of objects is
        /// returned. Sort conditions are mandatory when paging parameters are specified.
        /// <a href="https://stackoverflow.com/questions/21112654/dynamic-expression-for-generic-orderby-clause">Dynamic expression for generic OrderBy clause</a>
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="ArgumentException">A sortCondition is required if pagingContext is provided.</exception>"
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the objects.</exception>
        IEnumerable<TEntity> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Retrieve an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Instance if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the object.</exception>
        TEntity Retrieve(TKey id, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Retrieve all instances of the object type. If paging parameters are provided, a range of objects is
        /// returned. Sort conditions are mandatory when paging parameters are specified.
        /// <a href="https://stackoverflow.com/questions/21112654/dynamic-expression-for-generic-orderby-clause">Dynamic expression for generic OrderBy clause</a>
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="ArgumentException">A sortCondition is required if pagingContext is provided.</exception>"
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the objects.</exception>
        Task<IEnumerable<TEntity>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Retrieve an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Instance if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the object.</exception>
        Task<TEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);
    }
}