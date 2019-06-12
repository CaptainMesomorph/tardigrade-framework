using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFramework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the IRepository type.
    /// </summary>
    public static class RepositoryExtension
    {
        /// <summary>
        /// Create an instance of the object type.
        /// </summary>
        /// <param name="repository">Repository type being extended.</param>
        /// <param name="obj">Instance to create.</param>
        /// <param name="unitOfWork">Unit of Work used to define the transaction boundary.</param>
        /// <returns>Instance created (including allocated unique identifier).</returns>
        /// <exception cref="ArgumentException">The unitOfWork parameter is null or does not hold a database context.</exception>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="RepositoryException">Error creating the object.</exception>
        /// <exception cref="ValidationException">Object to create contains invalid values.</exception>
        public static T Create<T, PK>(
#pragma warning disable IDE0060 // Remove unused parameter
            this IRepository<T, PK> repository,
#pragma warning restore IDE0060 // Remove unused parameter
            T obj,
            EntityFrameworkUnitOfWork unitOfWork) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (unitOfWork?.DbContext == null)
            {
                throw new ArgumentException(nameof(unitOfWork));
            }

            try
            {
                unitOfWork.DbContext.Set<T>().Add(obj);
                unitOfWork.DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException($"Error creating an object of type {typeof(T).Name} as it contains invalid values.", e);
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error creating an object of type {typeof(T).Name}.", e);
            }

            return obj;
        }

        /// <summary>
        /// Delete an instance using their unique identifier.
        /// </summary>
        /// <param name="repository">Repository type being extended.</param>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="unitOfWork">Unit of Work used to define the transaction boundary.</param>
        /// <exception cref="ArgumentException">The unitOfWork parameter is null or does not hold a database context.</exception>
        /// <exception cref="ArgumentNullException">The id parameter is null.</exception>
        /// <exception cref="RepositoryException">Error deleting the object.</exception>
        public static void Delete<T, PK>(
#pragma warning disable IDE0060 // Remove unused parameter
            this IRepository<T, PK> repository,
#pragma warning restore IDE0060 // Remove unused parameter
            PK id,
            EntityFrameworkUnitOfWork unitOfWork) where T : class
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (unitOfWork?.DbContext == null)
            {
                throw new ArgumentException(nameof(unitOfWork));
            }

            try
            {
                T obj = unitOfWork.DbContext.Set<T>().Find(id);
                unitOfWork.DbContext.Set<T>().Remove(obj);
                unitOfWork.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} with unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// Delete an instance.
        /// </summary>
        /// <param name="repository">Repository type being extended.</param>
        /// <param name="obj">Instance to delete.</param>
        /// <param name="unitOfWork">Unit of Work used to define the transaction boundary.</param>
        /// <exception cref="ArgumentException">The unitOfWork parameter is null or does not hold a database context.</exception>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="RepositoryException">Error deleting the object.</exception>
        public static void Delete<T, PK>(
#pragma warning disable IDE0060 // Remove unused parameter
            this IRepository<T, PK> repository,
#pragma warning restore IDE0060 // Remove unused parameter
            T obj,
            EntityFrameworkUnitOfWork unitOfWork) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (unitOfWork?.DbContext == null)
            {
                throw new ArgumentException(nameof(unitOfWork));
            }

            try
            {
                if (unitOfWork.DbContext.Entry(obj).State == EntityState.Detached)
                {
                    unitOfWork.DbContext.Set<T>().Attach(obj);
                }

                unitOfWork.DbContext.Set<T>().Remove(obj);
                unitOfWork.DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// Update an instance.
        /// </summary>
        /// <param name="repository">Repository type being extended.</param>
        /// <param name="obj">Instance to update.</param>
        /// <param name="unitOfWork">Unit of Work used to define the transaction boundary.</param>
        /// <exception cref="ArgumentException">The unitOfWork parameter is null or does not hold a database context.</exception>
        /// <exception cref="ArgumentNullException">The obj parameter is null.</exception>
        /// <exception cref="RepositoryException">Error updating the object.</exception>
        /// <exception cref="ValidationException">Object to update contains invalid values.</exception>
        public static void Update<T, PK>(
#pragma warning disable IDE0060 // Remove unused parameter
            this IRepository<T, PK> repository,
#pragma warning restore IDE0060 // Remove unused parameter
            T obj,
            EntityFrameworkUnitOfWork unitOfWork) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (unitOfWork?.DbContext == null)
            {
                throw new ArgumentException(nameof(unitOfWork));
            }

            try
            {
                unitOfWork.DbContext.Set<T>().Attach(obj);
                unitOfWork.DbContext.Entry(obj).State = EntityState.Modified;
                unitOfWork.DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException($"Error updating an object of type {typeof(T).Name} as it contains invalid values.", e);
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error updating an object of type {typeof(T).Name}.", e);
            }
        }
    }
}