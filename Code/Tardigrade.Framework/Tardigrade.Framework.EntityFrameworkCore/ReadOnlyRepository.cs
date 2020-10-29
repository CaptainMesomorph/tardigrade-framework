using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFrameworkCore
{
    /// <summary>
    /// <see cref="IReadOnlyRepository{TEntity, TKey}"/>
    /// <a href="https://stackoverflow.com/questions/40132380/ef-cannot-apply-operator-to-operands-of-type-tid-and-tid">EF - Cannot apply operator '==' to operands of type 'TId' and 'TId'</a>
    /// </summary>
    public class ReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
        where TEntity : class, IHasUniqueIdentifier<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Database context.
        /// </summary>
        protected DbContext DbContext { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>"
        /// <exception cref="RepositoryException">Object type TEntity is not recognised in the dbContext.</exception>
        public ReadOnlyRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            if (DbContext.Set<TEntity>() == null)
            {
                throw new RepositoryException(
                    $"Object type {typeof(TEntity).Name} is not recognised in the DbContext.");
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Count(Expression{Func{TEntity, bool}})"/>
        /// </summary>
        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            try
            {
                return filter == null ? DbContext.Set<TEntity>().Count() : DbContext.Set<TEntity>().Count(filter);
            }
            catch (OverflowException e)
            {
                throw new RepositoryException(
                    $"Count failed; more than {int.MaxValue} objects of type {typeof(TEntity).Name} exist.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.CountAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return filter == null
                    ? await DbContext.Set<TEntity>().CountAsync(cancellationToken)
                    : await DbContext.Set<TEntity>().CountAsync(filter, cancellationToken);
            }
            catch (OverflowException e)
            {
                throw new RepositoryException(
                    $"Count failed; more than {int.MaxValue} objects of type {typeof(TEntity).Name} exist.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Exists(TKey)"/>
        /// </summary>
        public virtual bool Exists(TKey id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return DbContext.Set<TEntity>().Any(o => o.Id.Equals(id));
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.ExistsAsync(TKey, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await DbContext.Set<TEntity>().AnyAsync(o => o.Id.Equals(id), cancellationToken);
        }

        /// <summary>
        /// Generate a query to retrieve an instance of the object type.
        /// </summary>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Query to retrieve an instance of the object type.</returns>
        private IQueryable<TEntity> FindQuery(params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = DbContext.Set<TEntity>();

            foreach (Expression<Func<TEntity, object>> include in includes.OrEmptyIfNull())
            {
                query = query.Include(include);
            }

            return query;
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Retrieve(Expression{Func{TEntity, bool}}, PagingContext, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<TEntity> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return RetrieveQuery(filter, pagingContext, sortCondition, includes).ToList();
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.Retrieve(TKey, Expression{Func{TEntity, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
        /// </summary>
        public virtual TEntity Retrieve(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            TEntity item = includes.IsNullOrEmpty() ?
                DbContext.Set<TEntity>().Find(id) : FindQuery(includes).SingleOrDefault(o => o.Id.Equals(id));

            return item;
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.RetrieveAsync(Expression{Func{TEntity, bool}}, PagingContext, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}, CancellationToken, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        public virtual async Task<IEnumerable<TEntity>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            return await RetrieveQuery(filter, pagingContext, sortCondition, includes).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{TEntity, TKey}.RetrieveAsync(TKey, CancellationToken, Expression{Func{TEntity, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
        /// </summary>
        public virtual async Task<TEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            TEntity item;

            if (includes.IsNullOrEmpty())
            {
                item = await DbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
            }
            else
            {
                item = await FindQuery(includes).SingleOrDefaultAsync(o => o.Id.Equals(id), cancellationToken);
            }

            return item;
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
        private IQueryable<TEntity> RetrieveQuery(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            if (pagingContext != null && sortCondition == null)
            {
                throw new ArgumentException(
                    $"{nameof(sortCondition)} is required if {nameof(pagingContext)} is provided.");
            }

            IQueryable<TEntity> query = DbContext.Set<TEntity>();

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
                    query = query
                        .Skip(skip)
                        .Take((int)pagingContext.PageSize);
                }
            }

            foreach (Expression<Func<TEntity, object>> include in includes.OrEmptyIfNull())
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}