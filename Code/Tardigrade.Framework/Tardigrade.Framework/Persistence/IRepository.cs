using System;
using System.Linq.Expressions;

namespace Tardigrade.Framework.Persistence
{
    /// <summary>
    /// Base repository interface for objects of a particular type.
    /// </summary>
    /// <typeparam name="T">Object type associated with the repository operations.</typeparam>
    /// <typeparam name="PK">Unique identifier type for the object type.</typeparam>
    public interface IRepository<T, PK> : ICrudRepository<T, PK>
    {
        /// <summary>
        /// Calculate the number of objects in the repository.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <returns>Number of objects in the repository.</returns>
        /// <exception cref="Exceptions.RepositoryException">Error calculating the number of objects.</exception>
        int Count(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Check for existence of an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.RepositoryException">Error checking for existence of an object.</exception>
        bool Exists(PK id);
    }
}