using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Models.Persistence;
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
        /// <see cref="IObjectService{T, PK}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            try
            {
                return Repository.Count(filter);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error calculating the number of objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Create(T)"/>
        /// </summary>
        public virtual T Create(T obj)
        {
            try
            {
                return Repository.Create(obj);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
            }
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
        /// <see cref="IObjectService{T, PK}.Exists(PK)"/>
        /// </summary>
        public virtual bool Exists(PK id)
        {
            try
            {
                return Repository.Exists(id);
            }
            catch (Exception e)
            {
                throw new ServiceException($"Error determing whether an object of type {typeof(T).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<T> Retrieve(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                return Repository.Retrieve(filter, pagingContext, sortCondition, includes);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Retrieve(PK)"/>
        /// </summary>
        public virtual T Retrieve(PK id)
        {
            try
            {
                return Repository.Retrieve(id);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(T).Name} with a unique identifier of {id}.", e);
            }
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