using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
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
        /// <see cref="IObjectService{T, PK}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.CountAsync(filter, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error calculating the number of objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Create(IEnumerable{T})"/>
        /// </summary>
        public virtual IEnumerable<T> Create(IEnumerable<T> objs)
        {
            try
            {
                return Repository.CreateBulk(objs);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
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
        /// <see cref="IObjectService{T, PK}.CreateAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<IEnumerable<T>> CreateAsync(
            IEnumerable<T> objs,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.CreateBulkAsync(objs, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task<T> CreateAsync(T obj, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.CreateAsync(obj, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
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
        /// <see cref="IObjectService{T, PK}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(T obj, CancellationToken cancellationToken = default)
        {
            try
            {
                await Repository.DeleteAsync(obj, cancellationToken);
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
        /// <see cref="IObjectService{T, PK}.ExistsAsync(PK, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(PK id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.ExistsAsync(id, cancellationToken);
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
        /// <see cref="IObjectService{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual T Retrieve(PK id, params Expression<Func<T, object>>[] includes)
        {
            try
            {
                return Repository.Retrieve(id, includes);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(T).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual async Task<IEnumerable<T>> RetrieveAsync(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                return await Repository.RetrieveAsync(filter, pagingContext, sortCondition, cancellationToken, includes);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.RetrieveAsync(PK, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual async Task<T> RetrieveAsync(
            PK id,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includes)
        {
            try
            {
                return await Repository.RetrieveAsync(id, cancellationToken, includes);
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

        /// <summary>
        /// <see cref="IObjectService{T, PK}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(T obj, CancellationToken cancellationToken = default)
        {
            try
            {
                await Repository.UpdateAsync(obj, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(T).Name}.", e);
            }
        }
    }
}