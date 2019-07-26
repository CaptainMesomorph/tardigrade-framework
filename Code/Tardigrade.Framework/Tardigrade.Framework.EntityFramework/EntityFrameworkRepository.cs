﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFramework
{
    /// <summary>
    /// <see cref="IRepository{T, PK}"/>
    /// </summary>
    public class EntityFrameworkRepository<T, PK> : IRepository<T, PK> where T : class
    {
        /// <summary>
        /// Database context.
        /// </summary>
        protected DbContext DbContext { get; private set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>"
        /// <exception cref="RepositoryException">Object type T is not recognised in the dbContext.</exception>
        public EntityFrameworkRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            if (DbContext.Set<T>() == null)
            {
                throw new RepositoryException($"Object type {typeof(T).Name} is not recognised in the DbContext.");
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        public virtual int Count(Expression<Func<T, bool>> filter = null)
        {
            try
            {
                if (filter == null)
                {
                    return DbContext.Set<T>().Count();
                }
                else
                {
                    return DbContext.Set<T>().Count(filter);
                }
            }
            catch (OverflowException e)
            {
                throw new RepositoryException($"Count failed; more than {int.MaxValue} objects of type {typeof(T).Name} exist.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (filter == null)
                {
                    return await DbContext.Set<T>().CountAsync(cancellationToken);
                }
                else
                {
                    return await DbContext.Set<T>().CountAsync(filter, cancellationToken);
                }
            }
            catch (OverflowException e)
            {
                throw new RepositoryException($"Count failed; more than {int.MaxValue} objects of type {typeof(T).Name} exist.", e);
            }
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
                    throw new AlreadyExistsException($"Create failed; object of type {typeof(T).Name} with the same primary key already exists.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Create failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Add(obj);

            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException($"Create failed; object of type {typeof(T).Name} contains invalid values.", e);
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException($"Create failed; database error while creating object of type {typeof(T).Name}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task<T> CreateAsync(
            T obj,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (Exists(obj))
                {
                    throw new AlreadyExistsException($"Create failed; object of type {typeof(T).Name} with the same primary key already exists.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Create failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Add(obj);

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Create failed; database connection error while creating object of type {typeof(T).Name}.", e);
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
                throw new RepositoryException($"Create failed; database error while creating objects of type {typeof(T).Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// <see cref="IBulkRepository{T}.CreateBulkAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<IEnumerable<T>> CreateBulkAsync(IEnumerable<T> objs, CancellationToken cancellationToken = default)
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
                throw new RepositoryException($"Create failed; database connection error while creating objects of type {typeof(T).Name}.", e);
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
                    throw new NotFoundException($"Delete failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Delete failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
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
                throw new RepositoryException($"Delete failed; database error while deleting object of type {typeof(T).Name}.", e);
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

            try
            {
                if (!Exists(obj))
                {
                    throw new NotFoundException($"Delete failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Delete failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
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
                throw new RepositoryException($"Delete failed; database connection error while deleting object of type {typeof(T).Name}.", e);
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
                throw new RepositoryException($"Delete failed; database error while deleting objects of type {typeof(T).Name}.", e);
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
                throw new RepositoryException($"Delete failed; database connection error while deleting objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Exists(PK)"/>
        /// </summary>
        public virtual bool Exists(PK id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                return (DbContext.Set<T>().Find(id) != null);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Exists failed; unable to determine whether object of type {typeof(T).Name} with primary key {id} exists.", e);
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
        /// <see cref="IRepository{T, PK}.ExistsAsync(PK, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            PK id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            T obj = await DbContext.Set<T>().FindAsync(id);

            return (obj != null);
        }

        /// <summary>
        /// Generate a query to retrieve a set of objects.
        /// NOTE: Not currently used. Awaiting changes to add an Id property to the model.
        /// </summary>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Query to retrieve a set of objects.</returns>
        private IQueryable<T> FindQuery(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbContext.Set<T>();

            foreach (Expression<Func<T, object>> include in includes.OrEmptyIfNull())
            {
                query = query.Include(include);
            }

            return query;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<T> Retrieve(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            params Expression<Func<T, object>>[] includes)
        {
            return RetrieveQuery(filter, pagingContext, sortCondition, includes).ToList();
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
        /// <a href="https://docs.microsoft.com/en-au/ef/ef6/querying/related-data">Loading Related Entities</a>
        /// <a href="https://devblogs.microsoft.com/csharpfaq/how-can-i-get-objects-and-property-values-from-expression-trees/">How can I get objects and property values from expression trees?</a>
        /// </summary>
        public virtual T Retrieve(PK id, params Expression<Func<T, object>>[] includes)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                T obj = DbContext.Set<T>().Find(id);
                DbEntityEntry<T> entity = DbContext.Entry(obj);

                foreach (Expression<Func<T, object>> include in includes)
                {
                    try
                    {
                        entity.Reference(include).Load();
                    }
                    catch (ArgumentException)
                    {
                        MemberExpression expression = (MemberExpression)include.Body;
                        string name = expression.Member.Name;

                        try
                        {
                            entity.Collection(name).Load();
                        }
                        catch (ArgumentException e)
                        {
                            throw new RepositoryException($"Retrieve failed; a lazy-loading error occurred retrieving object of type {typeof(T).Name} with primary key {id}.", e);
                        }
                    }
                }

                return obj;
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Retrieve failed; database connection error while retrieving object of type {typeof(T).Name} with primary key {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual async Task<IEnumerable<T>> RetrieveAsync(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<T, object>>[] includes)
        {
            return await RetrieveQuery(filter, pagingContext, sortCondition, includes).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.RetrieveAsync(PK, CancellationToken, Expression{Func{T, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
        /// <a href="https://docs.microsoft.com/en-au/ef/ef6/querying/related-data">Loading Related Entities</a>
        /// <a href="https://devblogs.microsoft.com/csharpfaq/how-can-i-get-objects-and-property-values-from-expression-trees/">How can I get objects and property values from expression trees?</a>
        /// </summary>
        public virtual async Task<T> RetrieveAsync(
            PK id,
            CancellationToken cancellationToken = default(CancellationToken),
            params Expression<Func<T, object>>[] includes)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            T obj = await DbContext.Set<T>().FindAsync(cancellationToken, id);
            DbEntityEntry<T> entity = DbContext.Entry(obj);

            foreach (Expression<Func<T, object>> include in includes)
            {
                try
                {
                    await entity.Reference(include).LoadAsync(cancellationToken);
                }
                catch (ArgumentException)
                {
                    MemberExpression expression = (MemberExpression)include.Body;
                    string name = expression.Member.Name;

                    try
                    {
                        await entity.Collection(name).LoadAsync(cancellationToken);
                    }
                    catch (ArgumentException e)
                    {
                        throw new RepositoryException($"Retrieve failed; a lazy-loading error occurred retrieving object of type {typeof(T).Name} with primary key {id}.", e);
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// Generate a query to retrieve all instances of the object type.
        /// </summary>
        /// <param name="filter">Filter condition.</param>
        /// <param name="pagingContext">Paging parameters.</param>
        /// <param name="sortCondition">Condition used to define sorting.</param>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Query to retrieve all instances of the object type.</returns>
        /// <exception cref="ArgumentException">A sortCondition is required if pagingContext is provided.</exception>"
        private IQueryable<T> RetrieveQuery(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            params Expression<Func<T, object>>[] includes)
        {
            if (pagingContext != null && sortCondition == null)
            {
                throw new ArgumentException($"{nameof(sortCondition)} is required if {nameof(pagingContext)} is provided.");
            }

            IList<T> objs = new List<T>();
            IQueryable<T> query = DbContext.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (sortCondition != null)
            {
                query = sortCondition(query);

                if (pagingContext?.PageSize > 0)
                {
                    // Using a variable rather than a calculation in the EF/LINQ query is required as (for some
                    // unknown reason) the calculation result does not properly cast to an int.
                    int skip = (int)(pagingContext.PageIndex * pagingContext.PageSize);
                    // https://visualstudiomagazine.com/articles/2016/12/06/skip-take-entity-framework-lambda.aspx
                    query = query
                        .Skip(() => skip)
                        .Take(() => (int)pagingContext.PageSize);
                }
            }

            foreach (Expression<Func<T, object>> include in includes.OrEmptyIfNull())
            {
                query = query.Include(include);
            }

            return query;
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
                    throw new NotFoundException($"Update failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Update failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Attach(obj);
            DbContext.Entry(obj).State = EntityState.Modified;

            try
            {
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException($"Update failed; object of type {typeof(T).Name} contains invalid values.", e);
            }
            catch (Exception e) when (
                e is DbUpdateException ||
                e is DbUpdateConcurrencyException ||
                e is NotSupportedException ||
                e is ObjectDisposedException ||
                e is InvalidOperationException)
            {
                throw new RepositoryException($"Update failed; database error while updating object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(T obj, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (!Exists(obj))
                {
                    throw new NotFoundException($"Update failed; unable to find specified object of type {typeof(T).Name}.");
                }
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Update failed; object type {typeof(T).Name} does not have a recognised primary key.", e);
            }

            DbContext.Set<T>().Attach(obj);
            DbContext.Entry(obj).State = EntityState.Modified;

            try
            {
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException($"Update failed; database connection error while updating object of type {typeof(T).Name}.", e);
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
                throw new RepositoryException($"Update failed; database error while updating objects of type {typeof(T).Name}.", e);
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
                throw new RepositoryException($"Update failed; database error while updating objects of type {typeof(T).Name}.", e);
            }
            finally
            {
                DbContext.Configuration.AutoDetectChangesEnabled = true;
            }
        }
    }
}