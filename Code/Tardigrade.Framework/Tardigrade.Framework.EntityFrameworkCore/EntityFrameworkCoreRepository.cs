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
    /// <see cref="IRepository{T, PK}"/>
    /// <a href="https://stackoverflow.com/questions/40132380/ef-cannot-apply-operator-to-operands-of-type-tid-and-tid">EF - Cannot apply operator '==' to operands of type 'TId' and 'TId'</a>
    /// </summary>
    public class EntityFrameworkCoreRepository<T, PK>
        : EntityFrameworkCoreReadOnlyRepository<T, PK>, IRepository<T, PK>
        where T : class, IHasUniqueIdentifier<PK>
        where PK : IEquatable<PK>
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
        /// <see cref="IRepository{T, PK}.Create(T)"/>
        /// </summary>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual T Create(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Id != null && Exists(obj.Id))
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with primary key {obj.Id} already exists.");
            }

            DbContext.Set<T>().Add(obj);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException || e is DbUpdateConcurrencyException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(T).Name}.",
                    e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        /// <param name="obj">Instance to create.</param>
        /// <param name="cancellationToken">Not supported.</param>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task<T> CreateAsync(
            T obj,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Id != null && (await ExistsAsync(obj.Id)))
            {
                throw new AlreadyExistsException(
                    $"Create failed; object of type {typeof(T).Name} with primary key {obj.Id} already exists.");
            }

            await DbContext.Set<T>().AddAsync(obj, cancellationToken);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException || e is DbUpdateConcurrencyException)
            {
                throw new RepositoryException(
                    $"Create failed; database error while creating object of type {typeof(T).Name}.",
                    e);
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

            DbContext.BulkInsert((IList<T>)objs);

            return objs;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <param name="objs">Instances to create.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task<IEnumerable<T>> CreateBulkAsync(
            IEnumerable<T> objs,
            CancellationToken cancellationToken = default)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            await DbContext.BulkInsertAsync((IList<T>)objs);

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

            if (obj.Id == null || !Exists(obj.Id))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(T).Name} with primary key {obj.Id} does not exist.");
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
            catch (Exception e) when (e is DbUpdateException || e is DbUpdateConcurrencyException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(T).Name} with primary key {obj.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(T obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Id == null || !Exists(obj.Id))
            {
                throw new NotFoundException(
                    $"Delete failed; object of type {typeof(T).Name} with primary key {obj.Id} does not exist.");
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
            catch (Exception e) when (e is DbUpdateException || e is DbUpdateConcurrencyException)
            {
                throw new RepositoryException(
                    $"Delete failed; database error while deleting object of type {typeof(T).Name} with primary key {obj.Id}.",
                    e);
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

            DbContext.BulkDelete((IList<T>)objs);
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.DeleteBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <param name="objs">Instances to delete.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task DeleteBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            await DbContext.BulkDeleteAsync((IList<T>)objs);
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Update(T)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The obj parameter (or associated primary key) is null.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual void Update(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Id == null || !Exists(obj.Id))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(T).Name} with primary key {obj.Id} does not exist.");
            }

            DbContext.Set<T>().Update(obj);

            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e) when (e is DbUpdateException || e is DbUpdateConcurrencyException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(T).Name} with primary key {obj.Id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Update(T)"/>
        /// </summary>
        /// <exception cref="ArgumentNullException">The obj parameter (or associated primary key) is null.</exception>
        /// <exception cref="ValidationException">Not supported.</exception>
        public virtual async Task UpdateAsync(T obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj.Id == null || !await ExistsAsync(obj.Id))
            {
                throw new NotFoundException(
                    $"Update failed; object of type {typeof(T).Name} with primary key {obj.Id} does not exist.");
            }

            DbContext.Set<T>().Update(obj);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e) when (e is DbUpdateException || e is DbUpdateConcurrencyException)
            {
                throw new RepositoryException(
                    $"Update failed; database error while updating object of type {typeof(T).Name} with primary key {obj.Id}.",
                    e);
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

            DbContext.BulkUpdate((IList<T>)objs);
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.UpdateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        /// <param name="objs">Instances to update.</param>
        /// <param name="cancellationToken">Not supported.</param>
        public virtual async Task UpdateBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
        {
            if (objs.IsNulOrEmpty())
            {
                throw new ArgumentNullException(nameof(objs));
            }

            await DbContext.BulkUpdateAsync((IList<T>)objs);
        }
    }
}