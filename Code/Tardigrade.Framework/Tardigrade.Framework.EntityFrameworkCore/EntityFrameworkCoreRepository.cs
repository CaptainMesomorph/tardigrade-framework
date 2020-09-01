using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFrameworkCore
{
    /// <summary>
    /// <see cref="IRepository{T, Pk}"/>
    /// <a href="https://stackoverflow.com/questions/40132380/ef-cannot-apply-operator-to-operands-of-type-tid-and-tid">EF - Cannot apply operator '==' to operands of type 'TId' and 'TId'</a>
    /// </summary>
    public class EntityFrameworkCoreRepository<T, Pk> : EntityFrameworkCoreReadOnlyRepository<T, Pk>, IRepository<T, Pk>
        where T : class, IHasUniqueIdentifier<Pk>
        where Pk : IEquatable<Pk>
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>"
        /// <exception cref="RepositoryException">Object type T is not recognised in the dbContext.</exception>
        public EntityFrameworkCoreRepository(DbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// <see cref="IRepository{T, Pk}.Create(T)"/>
        /// </summary>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual T Create(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id != null && Exists(item.Id))
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with primary key {item.Id} already exists.");
            }

            DbContext.Set<T>().Add(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(T).Name}.",
                    e);
            }

            return item;
        }

        /// <summary>
        /// <see cref="IRepository{T, Pk}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        /// <param name="item">Instance to create.</param>
        /// <param name="cancellationToken">Not supported.</param>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task<T> CreateAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id != null && (await ExistsAsync(item.Id, cancellationToken)))
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with primary key {item.Id} already exists.");
            }

            await DbContext.Set<T>().AddAsync(item, cancellationToken);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(T).Name}.",
                    e);
            }

            return item;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulk(IEnumerable{T})"/>
        /// </summary>
        public virtual IEnumerable<T> CreateBulk(IEnumerable<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            DbContext.BulkInsert((IList<T>)items);

            return items;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <param name="items">Instances to create.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task<IEnumerable<T>> CreateBulkAsync(
            IEnumerable<T> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            await DbContext.BulkInsertAsync((IList<T>)items, cancellationToken: cancellationToken);

            return items;
        }

        /// <summary>
        /// <see cref="IRepository{T, Pk}.Delete(T)"/>
        /// </summary>
        public virtual void Delete(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id == null || !Exists(item.Id))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(T).Name} with primary key {item.Id} does not exist.");
            }

            if (DbContext.Entry(item).State == EntityState.Detached)
            {
                DbContext.Set<T>().Attach(item);
            }

            DbContext.Set<T>().Remove(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(T).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, Pk}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id == null || !await ExistsAsync(item.Id, cancellationToken))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(T).Name} with primary key {item.Id} does not exist.");
            }

            if (DbContext.Entry(item).State == EntityState.Detached)
            {
                DbContext.Set<T>().Attach(item);
            }

            DbContext.Set<T>().Remove(item);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(T).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.DeleteBulk(IEnumerable{T})"/>
        /// </summary>
        public virtual void DeleteBulk(IEnumerable<T> items)
        {
            if (items.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(items));
            }

            DbContext.BulkDelete((IList<T>)items);
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.DeleteBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <param name="items">Instances to delete.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task DeleteBulkAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            if (items.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(items));
            }

            await DbContext.BulkDeleteAsync((IList<T>)items, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{T, Pk}.Update(T)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter (or associated primary key) is null.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual void Update(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id == null || !Exists(item.Id))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(T).Name} with primary key {item.Id} does not exist.");
            }

            DbContext.Set<T>().Update(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(T).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, Pk}.Update(T)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter (or associated primary key) is null.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task UpdateAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Id == null || !await ExistsAsync(item.Id, cancellationToken))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(T).Name} with primary key {item.Id} does not exist.");
            }

            DbContext.Set<T>().Update(item);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(T).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.UpdateBulk(IEnumerable{T})"/>
        /// </summary>
        public virtual void UpdateBulk(IEnumerable<T> items)
        {
            if (items.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(items));
            }

            DbContext.BulkUpdate((IList<T>)items);
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.UpdateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <param name="items">Instances to update.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task UpdateBulkAsync(IEnumerable<T> items, CancellationToken cancellationToken = default)
        {
            if (items.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(items));
            }

            await DbContext.BulkUpdateAsync((IList<T>)items, cancellationToken: cancellationToken);
        }
    }
}