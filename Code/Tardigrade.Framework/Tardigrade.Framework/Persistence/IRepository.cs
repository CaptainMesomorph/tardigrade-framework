﻿using System;
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
    /// <typeparam name="T">Object type associated with the repository operations.</typeparam>
    /// <typeparam name="PK">Unique identifier type for the object type.</typeparam>
    public interface IRepository<T, PK>
    {
        /// <summary>
        /// Calculate the number of objects in the repository.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <returns>Number of objects in the repository.</returns>
        /// <exception cref="Exceptions.RepositoryException">Error calculating the number of objects.</exception>
        int Count(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Calculate the number of objects in the repository.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Number of objects in the repository.</returns>
        /// <exception cref="Exceptions.RepositoryException">Error calculating the number of objects.</exception>
        Task<int> CountAsync(
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create multiple instances of the object type.
        /// </summary>
        /// <param name="objs">Instances to create.</param>
        /// <returns>Instances created (including allocated unique identifiers).</returns>
        /// <exception cref="ArgumentNullException">The objs parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the objects.</exception>
        IEnumerable<T> Create(IEnumerable<T> objs);

        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="obj">Instance to create.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="Exceptions.AlreadyExistsException">Object already exists.</exception>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to create contains invalid values.</exception>
        T Create(T obj);

        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="obj">Instance to create.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="Exceptions.AlreadyExistsException">Object already exists.</exception>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to create contains invalid values.</exception>
        Task<T> CreateAsync(T obj, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        void Delete(PK id);

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="obj">Instance to delete.</param>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        void Delete(T obj);

        /// <summary>
        /// Delete an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        Task DeleteAsync(PK id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="obj">Instance to delete.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        Task DeleteAsync(T obj, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Check for existence of an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error checking for existence of an object.</exception>
        bool Exists(PK id);

        /// <summary>
        /// Check for existence of an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error checking for existence of an object.</exception>
        Task<bool> ExistsAsync(PK id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Retrieve all instances of the object type. If paging parameters are provided, a range of objects is
        /// returned. Sort conditions are mandatory when paging parameters are specified.
        /// <a href="https://stackoverflow.com/questions/21112654/dynamic-expression-for-generic-orderby-clause">Dynamic expression for generic orderby clause</a>
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="ArgumentException">A sortCondition is required if pagingContext is provided.</exception>"
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the objects.</exception>
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
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the object.</exception>
        T Retrieve(PK id, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Retrieve all instances of the object type. If paging parameters are provided, a range of objects is
        /// returned. Sort conditions are mandatory when paging parameters are specified.
        /// <a href="https://stackoverflow.com/questions/21112654/dynamic-expression-for-generic-orderby-clause">Dynamic expression for generic orderby clause</a>
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="ArgumentException">A sortCondition is required if pagingContext is provided.</exception>"
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the objects.</exception>
        Task<IEnumerable<T>> RetrieveAsync(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Retrieve an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Instance if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the object.</exception>
        Task<T> RetrieveAsync(
            PK id,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="obj">Instance to update.</param>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error updating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to update contains invalid values.</exception>
        void Update(T obj);

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="obj">Instance to update.</param>
        /// <param name="cancellationToken">A CancellationToken to observe while waiting for the task to complete.</param>
        /// <returns>Task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to delete does not exist.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error updating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to update contains invalid values.</exception>
        Task UpdateAsync(T obj, CancellationToken cancellationToken = default(CancellationToken));
    }
}