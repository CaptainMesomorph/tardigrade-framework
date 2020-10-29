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
    public class ObjectService<TEntity, TKey> : IObjectService<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Bulk operations repository associated with the service.
        /// </summary>
        protected IBulkRepository<TEntity> BulkRepository { get; }

        /// <summary>
        /// Repository associated with the service.
        /// </summary>
        protected IRepository<TEntity, TKey> Repository { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        public ObjectService(IRepository<TEntity, TKey> repository)
        {
            Repository = repository;
            BulkRepository = repository as IBulkRepository<TEntity>;
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                return Repository.Count(filter);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error calculating the number of objects of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.CountAsync(filter, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error calculating the number of objects of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Create(IEnumerable{T})"/>
        /// <exception cref="NotSupportedException">Underlying repository layer does not support bulk operations.</exception>
        /// </summary>
        public virtual IEnumerable<TEntity> Create(IEnumerable<TEntity> items)
        {
            if (BulkRepository == null)
                throw new NotSupportedException("Underlying repository layer does not support bulk operations.");

            try
            {
                return BulkRepository.CreateBulk(items);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Create(T)"/>
        /// </summary>
        public virtual TEntity Create(TEntity item)
        {
            try
            {
                return Repository.Create(item);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.CreateAsync(IEnumerable{T}, CancellationToken)"/>
        /// <exception cref="NotSupportedException">Underlying repository layer does not support bulk operations.</exception>
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> CreateAsync(
            IEnumerable<TEntity> items,
            CancellationToken cancellationToken = default)
        {
            if (BulkRepository == null)
                throw new NotSupportedException("Underlying repository layer does not support bulk operations.");

            try
            {
                return await BulkRepository.CreateBulkAsync(items, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task<TEntity> CreateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.CreateAsync(item, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Delete(T)"/>
        /// </summary>
        public virtual void Delete(TEntity item)
        {
            try
            {
                Repository.Delete(item);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            try
            {
                await Repository.DeleteAsync(item, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Exists(Pk)"/>
        /// </summary>
        public virtual bool Exists(TKey id)
        {
            try
            {
                return Repository.Exists(id);
            }
            catch (Exception e)
            {
                throw new ServiceException($"Error determining whether an object of type {typeof(TEntity).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.ExistsAsync(Pk, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                return await Repository.ExistsAsync(id, cancellationToken);
            }
            catch (Exception e)
            {
                throw new ServiceException($"Error determining whether an object of type {typeof(TEntity).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<TEntity> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                return Repository.Retrieve(filter, pagingContext, sortCondition, includes);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Retrieve(Pk, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual TEntity Retrieve(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                return Repository.Retrieve(id, includes);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(TEntity).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                return await Repository.RetrieveAsync(filter, pagingContext, sortCondition, cancellationToken, includes);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.RetrieveAsync(Pk, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual async Task<TEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                return await Repository.RetrieveAsync(id, cancellationToken, includes);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(TEntity).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.Update(T)"/>
        /// </summary>
        public virtual void Update(TEntity item)
        {
            try
            {
                Repository.Update(item);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(TEntity).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, Pk}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            try
            {
                await Repository.UpdateAsync(item, cancellationToken);
            }
            catch (RepositoryException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(TEntity).Name}.", e);
            }
        }
    }
}