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
    /// <see cref="IObjectService{T, Pk}"/>
    /// </summary>
    public class ObjectService<T, Pk> : IObjectService<T, Pk> where T : class
    {
        /// <summary>
        /// Repository associated with the service.
        /// </summary>
        protected IRepository<T, Pk> Repository { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        public ObjectService(IRepository<T, Pk> repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Count(Expression{Func{T, bool}})"/>
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
        /// <see cref="IObjectService{T, Pk}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
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
        /// <see cref="IObjectService{T, Pk}.Create(IEnumerable{T})"/>
        /// </summary>
        public virtual IEnumerable<T> Create(IEnumerable<T> items)
        {
            try
            {
                return Repository.CreateBulk(items);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Create(T)"/>
        /// </summary>
        public virtual T Create(T item)
        {
            try
            {
                return Repository.Create(item);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.CreateAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<IEnumerable<T>> CreateAsync(
            IEnumerable<T> items,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.CreateBulkAsync(items, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task<T> CreateAsync(T item, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.CreateAsync(item, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Delete(T)"/>
        /// </summary>
        public virtual void Delete(T item)
        {
            try
            {
                Repository.Delete(item);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(T item, CancellationToken cancellationToken = default)
        {
            try
            {
                await Repository.DeleteAsync(item, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Exists(Pk)"/>
        /// </summary>
        public virtual bool Exists(Pk id)
        {
            try
            {
                return Repository.Exists(id);
            }
            catch (Exception e)
            {
                throw new ServiceException($"Error determining whether an object of type {typeof(T).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.ExistsAsync(Pk, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(Pk id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.ExistsAsync(id, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ServiceException($"Error determining whether an object of type {typeof(T).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
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
        /// <see cref="IObjectService{T, Pk}.Retrieve(Pk, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual T Retrieve(Pk id, params Expression<Func<T, object>>[] includes)
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
        /// <see cref="IObjectService{T, Pk}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
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
        /// <see cref="IObjectService{T, Pk}.RetrieveAsync(Pk, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual async Task<T> RetrieveAsync(
            Pk id,
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
        /// <see cref="IObjectService{T, Pk}.Update(T)"/>
        /// </summary>
        public virtual void Update(T item)
        {
            try
            {
                Repository.Update(item);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(T item, CancellationToken cancellationToken = default)
        {
            try
            {
                await Repository.UpdateAsync(item, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(T).Name}.", e);
            }
        }
    }
}