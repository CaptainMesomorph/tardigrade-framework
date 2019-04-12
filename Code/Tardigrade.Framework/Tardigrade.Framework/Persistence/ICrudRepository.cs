using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Models.Persistence;

namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Base repository interface of CRUD (create, retrieve, update and delete) operations for objects of a particular
    /// type.
    /// <a href="https://cpratt.co/truly-generic-repository/">A Truly Generic Repository, Part 1</a>
    /// </summary>
    /// <typeparam name="T">Object type associated with the repository operations.</typeparam>
    /// <typeparam name="PK">Unique identifier type for the object type.</typeparam>
    public interface ICrudRepository<T, PK>
    {
        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="obj">Instance to create.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error creating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to create contains invalid values.</exception>
        T Create(T obj);

        /// <summary>
        /// Delete an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        void Delete(PK id);

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="obj">Instance to delete.</param>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error deleting the object.</exception>
        void Delete(T obj);

        /// <summary>
        /// Retrieve all instances of the object type. If paging parameters are provided, a range of objects is
        /// returned.
        /// <a href="https://stackoverflow.com/questions/21112654/dynamic-expression-for-generic-orderby-clause">Dynamic expression for generic orderby clause</a>
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="ArgumentException">A pagingContext is specified but no sortCondition.</exception>"
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
        /// <param name="includes">A dot-separated list of related objects to include in the query results.</param>
        /// <returns>Instance if found; null otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the object.</exception>
        T Retrieve(PK id, string[] includes = null);

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="obj">Instance to update.</param>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error updating the object.</exception>
        /// <exception cref="Exceptions.ValidationException">Object to update contains invalid values.</exception>
        void Update(T obj);
    }
}