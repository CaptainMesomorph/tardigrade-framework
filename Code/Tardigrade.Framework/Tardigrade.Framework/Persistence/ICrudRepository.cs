using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Base repository interface of CRUD (create, retrieve, update and delete) operations for objects of a particular
    /// type.
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
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="pageIndex">The "page" index for the next set of objects to retrieve (starts at 0).</param>
        /// <param name="pageSize">The number of objects to retrieve for the "page".</param>
        /// <param name="includes">A dot-separated list of related objects to include in the query results.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="Exceptions.RepositoryException">Error retrieving the objects.</exception>
        IList<T> Retrieve(Expression<Func<T, bool>> predicate = null, int? pageIndex = null, int? pageSize = null, string[] includes = null);

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