using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFramework
{
    /// <summary>
    /// <see cref="IRepository{TEntity, Pk}"/>
    /// </summary>
    public class Repository<TEntity, TKey> : Repository<TEntity, TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>
        /// <exception cref="RepositoryException">Object type TEntity are not recognised in the dbContext.</exception>
        public Repository(DbContext dbContext) : base(dbContext)
        {
        }
    }

    /// <summary>
    /// Repository for a derived type from a type inheritance hierarchy.
    /// <see cref="IRepository{TEntity, PK}"/>
    /// </summary>
    /// <typeparam name="TBaseEntity">Base type associated with the repository operations.</typeparam>
    /// <typeparam name="TDerivedEntity">Derived type associated with the repository operations.</typeparam>
    /// <typeparam name="TKey">Unique identifier type for the object type.</typeparam>
    public class Repository<TBaseEntity, TDerivedEntity, TKey>
        : ReadOnlyRepository<TBaseEntity, TDerivedEntity, TKey>,
            IRepository<TDerivedEntity, TKey>,
            IBulkRepository<TDerivedEntity>
        where TBaseEntity : class
        where TDerivedEntity : class, TBaseEntity
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
        /// <see cref="IRepository{TEntity, Pk}.Create(TEntity)"/>
        /// </summary>
        public virtual TDerivedEntity Create(TDerivedEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                if (Exists(item))
                {
                    throw new AlreadyExistsException(
                        $"Create failed; object of type {typeof(TDerivedEntity).Name} with the same primary key already exists.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; object type {typeof(TDerivedEntity).Name} does not have a recognised primary key.",
                    e);
            }

            BaseEntityDbSet.Add(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException(
                    $"Create failed; object of type {typeof(TDerivedEntity).Name} contains invalid values.",
                    e);
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(TDerivedEntity).Name}.",
                    e);
            }

            return item;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, Pk}.CreateAsync(TEntity, CancellationToken)"/>
        /// </summary>
        public virtual async Task<TDerivedEntity> CreateAsync(TDerivedEntity item,
            CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                if (Exists(item))
                {
                    throw new AlreadyExistsException(
                        $"Create failed; object of type {typeof(TDerivedEntity).Name} with the same primary key already exists.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; object type {typeof(TDerivedEntity).Name} does not have a recognised primary key.",
                    e);
            }

            BaseEntityDbSet.Add(item);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; database connection error while creating object of type {typeof(TDerivedEntity).Name}.",
                    e);
            }

            return item;
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.CreateBulk(IEnumerable{TEntity})"/>
        /// </summary>
        public virtual IEnumerable<TDerivedEntity> CreateBulk(IEnumerable<TDerivedEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            BaseEntityDbSet.AddRange((IList<TDerivedEntity>)items);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating objects of type {typeof(TDerivedEntity).Name}.",
                    e);
            }

            return items;
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.CreateBulkAsync(IEnumerable{TEntity}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<IEnumerable<TDerivedEntity>> CreateBulkAsync(
            IEnumerable<TDerivedEntity> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            BaseEntityDbSet.AddRange((IList<TDerivedEntity>)items);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; database connection error while creating objects of type {typeof(TDerivedEntity).Name}.",
                    e);
            }

            return items;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, Pk}.Delete(TEntity)"/>
        /// </summary>
        public virtual void Delete(TDerivedEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                if (!Exists(item))
                {
                    throw new NotFoundException(
                        $"Delete failed; unable to find specified object of type {typeof(TDerivedEntity).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; object type {typeof(TDerivedEntity).Name} does not have a recognised primary key.",
                    e);
            }

            if (DbContext.Entry(item).State == EntityState.Detached)
            {
                BaseEntityDbSet.Attach(item);
            }

            BaseEntityDbSet.Remove(item);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, Pk}.DeleteAsync(TEntity, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(TDerivedEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                if (!Exists(item))
                {
                    throw new NotFoundException(
                        $"Delete failed; unable to find specified object of type {typeof(TDerivedEntity).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; object type {typeof(TDerivedEntity).Name} does not have a recognised primary key.",
                    e);
            }

            if (DbContext.Entry(item).State == EntityState.Detached)
            {
                BaseEntityDbSet.Attach(item);
            }

            BaseEntityDbSet.Remove(item);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; database connection error while deleting object of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.DeleteBulk(IEnumerable{TEntity})"/>
        /// </summary>
        public virtual void DeleteBulk(IEnumerable<TDerivedEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            BaseEntityDbSet.RemoveRange(items);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting objects of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.DeleteBulkAsync(IEnumerable{TEntity}, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteBulkAsync(IEnumerable<TDerivedEntity> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            BaseEntityDbSet.RemoveRange(items);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; database connection error while deleting objects of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
        }

        /// <summary>
        /// Check for existence of an instance.
        /// <a href="https://stackoverflow.com/questions/6018711/generic-way-to-check-if-entity-exists-in-entity-framework">Generic Way to Check If Entity Exists In Entity Framework?</a>
        /// </summary>
        /// <param name="item">Instance to check for existence. Assumed to be not null.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="InvalidOperationException">When the entity key cannot be constructed successfully for the existence check.</exception>
        private bool Exists(TDerivedEntity item)
        {
            ObjectContext objectContext = ((IObjectContextAdapter)DbContext).ObjectContext;
            ObjectSet<TBaseEntity> objectSet = objectContext.CreateObjectSet<TBaseEntity>();
            EntityKey entityKey = objectContext.CreateEntityKey(objectSet.EntitySet.Name, item);
            bool exists = objectContext.TryGetObjectByKey(entityKey, out object foundEntity);

            // TryGetObjectByKey attaches a found entity. Detach it here to prevent side-effects.
            if (exists)
            {
                objectContext.Detach(foundEntity);
            }

            return exists;
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, Pk}.Update(TEntity)"/>
        /// </summary>
        public virtual void Update(TDerivedEntity item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                if (!Exists(item))
                {
                    throw new NotFoundException(
                        $"Update failed; unable to find specified object of type {typeof(TDerivedEntity).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; object type {typeof(TDerivedEntity).Name} does not have a recognised primary key.",
                    e);
            }

            BaseEntityDbSet.Attach(item);
            DbContext.Entry(item).State = EntityState.Modified;

            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException(
                    $"Update failed; object of type {typeof(TDerivedEntity).Name} contains invalid values.",
                    e);
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{TEntity, Pk}.UpdateAsync(TEntity, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(TDerivedEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            try
            {
                if (!Exists(item))
                {
                    throw new NotFoundException(
                        $"Update failed; unable to find specified object of type {typeof(TDerivedEntity).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; object type {typeof(TDerivedEntity).Name} does not have a recognised primary key.",
                    e);
            }

            BaseEntityDbSet.Attach(item);
            DbContext.Entry(item).State = EntityState.Modified;

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; database connection error while updating object of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.UpdateBulk(IEnumerable{TEntity})"/>
        /// </summary>
        public virtual void UpdateBulk(IEnumerable<TDerivedEntity> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            DbContext.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                foreach (TDerivedEntity item in items)
                {
                    BaseEntityDbSet.Attach(item);
                    DbContext.Entry(item).State = EntityState.Modified;
                }

                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating objects of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
            finally
            {
                DbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{TEntity}.UpdateBulkAsync(IEnumerable{TEntity}, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateBulkAsync(IEnumerable<TDerivedEntity> items,
            CancellationToken cancellationToken = default)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            DbContext.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                foreach (TDerivedEntity item in items)
                {
                    BaseEntityDbSet.Attach(item);
                    DbContext.Entry(item).State = EntityState.Modified;
                }

                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating objects of type {typeof(TDerivedEntity).Name}.",
                    e);
            }
            finally
            {
                DbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }
    }
}