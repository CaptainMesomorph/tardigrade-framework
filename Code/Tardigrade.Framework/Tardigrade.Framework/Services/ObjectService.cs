using System.Collections.Generic;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.Services
{
    /// <summary>
    /// <see cref="IObjectService{T, PK}"/>
    /// </summary>
    public class ObjectService<T, PK> : IObjectService<T, PK> where T : class
    {
        /// <summary>
        /// Repository associated with the service.
        /// </summary>
        protected IRepository<T, PK> Repository { get; private set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        public ObjectService(IRepository<T, PK> repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Create(T)"/>
        /// </summary>
        public virtual T Create(T obj)
        {
            T createdObj;

            try
            {
                createdObj = Repository.Create(obj);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
            }

            return createdObj;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Delete(PK)"/>
        /// </summary>
        public virtual void Delete(PK id)
        {
            try
            {
                Repository.Delete(id);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(T).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Delete(T)"/>
        /// </summary>
        public virtual void Delete(T obj)
        {
            try
            {
                Repository.Delete(obj);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Retrieve(int?, int?)"/>
        /// </summary>
        public virtual IList<T> Retrieve(int? pageIndex = null, int? pageSize = null)
        {
            IList<T> objs = new List<T>();

            try
            {
                objs = Repository.Retrieve(pageIndex: pageIndex, pageSize: pageSize);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(T).Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Retrieve(PK)"/>
        /// </summary>
        public virtual T Retrieve(PK id)
        {
            T obj = default(T);

            try
            {
                obj = Repository.Retrieve(id);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(T).Name} with a unique identifier of {id}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Update(T)"/>
        /// </summary>
        public virtual void Update(T obj)
        {
            try
            {
                Repository.Update(obj);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(T).Name}.", e);
            }
        }
    }
}