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
    /// <see cref="IRepository{TEntity, TKey}"/>
    /// <a href="https://stackoverflow.com/questions/40132380/ef-cannot-apply-operator-to-operands-of-type-tid-and-tid">EF - Cannot apply operator '==' to operands of type 'TId' and 'TId'</a>
    /// </summary>
    public class Repository<TEntity, TKey>
        : ReadOnlyRepository<TEntity, TKey>, IRepository<TEntity, TKey>, IBulkRepository<TEntity>
        where TEntity : class, IHasUniqueIdentifier<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>"
        /// <exception cref="RepositoryException">Object type TEntity is not recognised in the dbContext.</exception>
        public Repository(DbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.Create(TEntity)"/>
        /// </summary>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual TEntity Create(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Id != null && Exists(item.Id))
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(TEntity).Name} with primary key {item.Id} already exists.");
            }

            DbContext.Set<TEntity>().Add(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(TEntity).Name}.",
                    e);
            }

            return item;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.CreateAsync(TEntity, CancellationToken)"/>
        /// </summary>
        /// <param name="item">Instance to create.</param>
        /// <param name="cancellationToken">Not supported.</param>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task<TEntity> CreateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Id != null && (await ExistsAsync(item.Id, cancellationToken)))
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(TEntity).Name} with primary key {item.Id} already exists.");
            }

            await DbContext.Set<TEntity>().AddAsync(item, cancellationToken);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(TEntity).Name}.",
                    e);
            }

            return item;
        }

        /// <summary>
        /// This method does not create the associated child entities. To do so, the child foreign keys need to be
        /// manually applied and the CreateBulk method called again on the child entities.
        /// <a href="https://github.com/borisdj/EFCore.BulkExtensions/blob/master/README.md#bulkconfig-arguments">BulkConfig arguments</a>
        /// <see cref="IBulkRepository{TEntity}.CreateBulk(IEnumerable{TEntity})"/>
        /// </summary>
        public virtual IEnumerable<TEntity> CreateBulk(IEnumerable<TEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var bulkConfig = new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true };
            DbContext.BulkInsert((IList<TEntity>)items, bulkConfig);

            return items;
        }

        /// <summary>
        /// This method does not create the associated child entities. To do so, the child foreign keys need to be
        /// manually applied and the CreateBulk method called again on the child entities.
        /// <a href="https://github.com/borisdj/EFCore.BulkExtensions/blob/master/README.md#bulkconfig-arguments">BulkConfig arguments</a>
        /// <see cref="IBulkRepository{TEntity}.CreateBulkAsync(IEnumerable{TEntity}, CancellationToken)"/>
        /// </summary>
        /// <param name="items">Instances to create.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task<IEnumerable<TEntity>> CreateBulkAsync(
            IEnumerable<TEntity> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var bulkConfig = new BulkConfig { PreserveInsertOrder = true, SetOutputIdentity = true };
            await DbContext.BulkInsertAsync((IList<TEntity>)items, bulkConfig, cancellationToken: cancellationToken);

            return items;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.Delete(TEntity)"/>
        /// </summary>
        public virtual void Delete(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Id == null || !Exists(item.Id))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(TEntity).Name} with primary key {item.Id} does not exist.");
            }

            if (DbContext.Entry(item).State == EntityState.Detached)
            {
                DbContext.Set<TEntity>().Attach(item);
            }

            DbContext.Set<TEntity>().Remove(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(TEntity).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.DeleteAsync(TEntity, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Id == null || !await ExistsAsync(item.Id, cancellationToken))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(TEntity).Name} with primary key {item.Id} does not exist.");
            }

            if (DbContext.Entry(item).State == EntityState.Detached)
            {
                DbContext.Set<TEntity>().Attach(item);
            }

            DbContext.Set<TEntity>().Remove(item);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(TEntity).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.DeleteBulk(IEnumerable{TEntity})"/>
        /// </summary>
        public virtual void DeleteBulk(IEnumerable<TEntity> items)
        {
            if (items.IsNullOrEmpty()) throw new ArgumentNullException(nameof(items));

            DbContext.BulkDelete((IList<TEntity>)items);
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.DeleteBulkAsync(IEnumerable{TEntity}, CancellationToken)"/>
        /// </summary>
        /// <param name="items">Instances to delete.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task DeleteBulkAsync(
            IEnumerable<TEntity> items,
            CancellationToken cancellationToken = default)
        {
            if (items.IsNullOrEmpty()) throw new ArgumentNullException(nameof(items));

            await DbContext.BulkDeleteAsync((IList<TEntity>)items, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.Update(TEntity)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter (or associated primary key) is null.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual void Update(TEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Id == null || !Exists(item.Id))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(TEntity).Name} with primary key {item.Id} does not exist.");
            }

            DbContext.Set<TEntity>().Update(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(TEntity).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, TKey}.Update(TEntity)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The item parameter (or associated primary key) is null.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            if (item.Id == null || !await ExistsAsync(item.Id, cancellationToken))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(TEntity).Name} with primary key {item.Id} does not exist.");
            }

            DbContext.Set<TEntity>().Update(item);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(TEntity).Name} with primary key {item.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.UpdateBulk(IEnumerable{TEntity})"/>
        /// </summary>
        public virtual void UpdateBulk(IEnumerable<TEntity> items)
        {
            if (items.IsNullOrEmpty()) throw new ArgumentNullException(nameof(items));

            DbContext.BulkUpdate((IList<TEntity>)items);
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.UpdateBulkAsync(IEnumerable{TEntity}, CancellationToken)"/>
        /// </summary>
        /// <param name="items">Instances to update.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task UpdateBulkAsync(
            IEnumerable<TEntity> items,
            CancellationToken cancellationToken = default)
        {
            if (items.IsNullOrEmpty()) throw new ArgumentNullException(nameof(items));

            await DbContext.BulkUpdateAsync((IList<TEntity>)items, cancellationToken: cancellationToken);
        }
    }
}