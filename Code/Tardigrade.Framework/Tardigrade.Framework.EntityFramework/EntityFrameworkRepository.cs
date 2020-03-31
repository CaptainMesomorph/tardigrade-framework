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
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFramework
{
    /// <summary>
    /// <see cref="IRepository{T, PK}"/>
    /// </summary>
    public class EntityFrameworkRepository<T, PK> : EntityFrameworkReadOnlyRepository<T, PK>, IRepository<T, PK>
        where T : class
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>"
        /// <exception cref="RepositoryException">Object type T is not recognised in the dbContext.</exception>
        public EntityFrameworkRepository(DbContext dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Create(T)"/>
        /// </summary>
        public virtual T Create(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (Exists(obj))
                {
                    throw new AlreadyExistsException(
                        $"Create failed; object of type {typeof(T).Name} with the same primary key already exists.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Add(obj);

            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException(
                    $"Create failed; object of type {typeof(T).Name} contains invalid values.", e);
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(T).Name}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task<T> CreateAsync(
            T obj,
            CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (Exists(obj))
                {
                    throw new AlreadyExistsException(
                        $"Create failed; object of type {typeof(T).Name} with the same primary key already exists.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Add(obj);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; database connection error while creating object of type {typeof(T).Name}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulk(IEnumerable{T})"/>
        /// </summary>
        public virtual IEnumerable<T> CreateBulk(IEnumerable<T> objs)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            DbContext.Set<T>().AddRange(objs);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating objects of type {typeof(T).Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<IEnumerable<T>> CreateBulkAsync(
            IEnumerable<T> objs,
            CancellationToken cancellationToken = default)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            DbContext.Set<T>().AddRange(objs);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Create failed; database connection error while creating objects of type {typeof(T).Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Delete(T)"/>
        /// </summary>
        public virtual void Delete(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (!Exists(obj))
                {
                    throw new NotFoundException(
                        $"Delete failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            if (DbContext.Entry(obj).State == EntityState.Detached)
            {
                DbContext.Set<T>().Attach(obj);
            }

            DbContext.Set<T>().Remove(obj);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(T obj, CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (!Exists(obj))
                {
                    throw new NotFoundException(
                        $"Delete failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            if (DbContext.Entry(obj).State == EntityState.Detached)
            {
                DbContext.Set<T>().Attach(obj);
            }

            DbContext.Set<T>().Remove(obj);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; database connection error while deleting object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.DeleteBulk(IEnumerable{T})"/>
        /// </summary>
        public virtual void DeleteBulk(IEnumerable<T> objs)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            DbContext.Set<T>().RemoveRange(objs);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.DeleteBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            DbContext.Set<T>().RemoveRange(objs);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Delete failed; database connection error while deleting objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// Check for existence of an instance.
        /// <a href="https://stackoverflow.com/questions/6018711/generic-way-to-check-if-entity-exists-in-entity-framework">Generic Way to Check If Entity Exists In Entity Framework?</a>
        /// </summary>
        /// <param name="obj">Instance to check for existence. Assumed to be not null.</param>
        /// <returns>True if the instance exists; false otherwise.</returns>
        /// <exception cref="InvalidOperationException">When the entity key cannot be constructed successfully for the existence check.</exception>
        private bool Exists(T obj)
        {
            ObjectContext objectContext = ((IObjectContextAdapter)DbContext).ObjectContext;
            ObjectSet<T> objectSet = objectContext.CreateObjectSet<T>();
            EntityKey entityKey = objectContext.CreateEntityKey(objectSet.EntitySet.Name, obj);
            bool exists = objectContext.TryGetObjectByKey(entityKey, out object foundEntity);

            // TryGetObjectByKey attaches a found entity. Detach it here to prevent side-effects.
            if (exists)
            {
                objectContext.Detach(foundEntity);
            }

            return exists;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Update(T)"/>
        /// </summary>
        public virtual void Update(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (!Exists(obj))
                {
                    throw new NotFoundException(
                        $"Update failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Attach(obj);
            DbContext.Entry(obj).State = EntityState.Modified;

            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException(
                    $"Update failed; object of type {typeof(T).Name} contains invalid values.", e);
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(T obj, CancellationToken cancellationToken = default)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (!Exists(obj))
                {
                    throw new NotFoundException(
                        $"Update failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Attach(obj);
            DbContext.Entry(obj).State = EntityState.Modified;

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; database connection error while updating object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.UpdateBulk(IEnumerable{T})"/>
        /// </summary>
        public virtual void UpdateBulk(IEnumerable<T> objs)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            DbContext.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                foreach (T obj in objs)
                {
                    DbContext.Set<T>().Attach(obj);
                    DbContext.Entry(obj).State = EntityState.Modified;
                }

                DbContext.SaveChanges();
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is DbEntityValidationException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating objects of type {typeof(T).Name}.", e);
            }
            finally
            {
                DbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.UpdateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            DbContext.Configuration.AutoDetectChangesEnabled = false;

            try
            {
                foreach (T obj in objs)
                {
                    DbContext.Set<T>().Attach(obj);
                    DbContext.Entry(obj).State = EntityState.Modified;
                }

                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating objects of type {typeof(T).Name}.", e);
            }
            finally
            {
                DbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }
    }
}