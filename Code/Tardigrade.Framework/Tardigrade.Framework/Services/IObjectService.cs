using System.Collections.Generic;

namespace Tardigrade.Framework.Services
{
    /// <summary>
    /// Base service interface for objects of a particular type.
    /// </summary>
    /// <typeparam name="T">Object type associated with the service operations.</typeparam>
    /// <typeparam name="PK">Unique identifier type for the object type.</typeparam>
    public interface IObjectService<T, PK>
    {
        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="obj">Instance to create.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="System.ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error creating the object.</exception>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">Object to create contains invalid values.</exception>
        T Create(T obj);

        /// <summary>
        /// Delete an instance using their unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <exception cref="System.ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error deleting the object.</exception>
        void Delete(PK id);

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="obj">Instance to delete.</param>
        /// <exception cref="System.ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error deleting the object.</exception>
        void Delete(T obj);

        /// <summary>
        /// Retrieve all instances fo the object type.
        /// </summary>
        /// <param name="pageIndex">The page index for the next set of objects to retrieve (starts at 0).</param>
        /// <param name="pageSize">The number of objects to retrieve for the page.</param>
        /// <returns>All instances if any; empty collection otherwise.</returns>
        /// <exception cref="Exceptions.ServiceException">Error retrieving the objects.</exception>
        IList<T> Retrieve(int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Retrieve an instance by unique identifier.
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <returns>Instance if found; null otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="Exceptions.ServiceException">Error retrieving the object.</exception>
        T Retrieve(PK id);

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="obj">Instance to update.</param>
        /// <exception cref="System.ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="Exceptions.NotFoundException">Object to update does not exist.</exception>
        /// <exception cref="Exceptions.ServiceException">Error updating the object.</exception>
        /// <exception cref="System.ComponentModel.DataAnnotations.ValidationException">Object to update contains invalid values.</exception>
        void Update(T obj);
    }
}