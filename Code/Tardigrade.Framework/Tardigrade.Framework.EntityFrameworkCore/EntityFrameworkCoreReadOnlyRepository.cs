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
    /// <see cref="IRepository{T, PK}"/>
    /// <a href="https://stackoverflow.com/questions/40132380/ef-cannot-apply-operator-to-operands-of-type-tid-and-tid">EF - Cannot apply operator '==' to operands of type 'TId' and 'TId'</a>
    /// </summary>
    public class EntityFrameworkCoreReadOnlyRepository<T, PK> : IReadOnlyRepository<T, PK>
        where T : class, IHasUniqueIdentifier<PK>
        where PK : IEquatable<PK>
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
        public EntityFrameworkCoreReadOnlyRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            if (DbContext.Set<T>() == null)
            {
                throw new RepositoryException($"Object type {typeof(T).Name} is not recognised in the DbContext.");
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.Count(Expression{Func{T, bool}})"/>
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
                throw new RepositoryException(
                    $"Count failed; more than {int.MaxValue} objects of type {typeof(T).Name} exist.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
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
                throw new RepositoryException(
                    $"Count failed; more than {int.MaxValue} objects of type {typeof(T).Name} exist.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.Exists(PK)"/>
        /// </summary>
        public virtual bool Exists(PK id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return DbContext.Set<T>().Any(o => o.Id.Equals(id));
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.ExistsAsync(PK, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            PK id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return await DbContext.Set<T>().AnyAsync(o => o.Id.Equals(id), cancellationToken);
        }

        /// <summary>
        /// Generate a query to retrieve an instance of the object type.
        /// </summary>
        /// <param name="includes">A list of related objects to include in the query results.</param>
        /// <returns>Query to retrieve an instance of the object type.</returns>
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
        /// <see cref="IReadOnlyRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
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
        /// <see cref="IReadOnlyRepository{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
        /// </summary>
        public virtual T Retrieve(PK id, params Expression<Func<T, object>>[] includes)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            T obj;

            if (includes.IsNulOrEmpty())
            {
                obj = DbContext.Set<T>().Find(id);
            }
            else
            {
                obj = FindQuery(includes).SingleOrDefault(o => o.Id.Equals(id));
            }

            return obj;
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
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
        /// <see cref="IReadOnlyRepository{T, PK}.RetrieveAsync(PK, CancellationToken, Expression{Func{T, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
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

            T obj;

            if (includes.IsNulOrEmpty())
            {
                obj = await DbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            }
            else
            {
                obj = await FindQuery(includes).SingleOrDefaultAsync(o => o.Id.Equals(id), cancellationToken);
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
                throw new ArgumentException(
                    $"{nameof(sortCondition)} is required if {nameof(pagingContext)} is provided.");
            }

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
                    query = query
                        .Skip(skip)
                        .Take((int)pagingContext.PageSize);
                }
            }

            foreach (Expression<Func<T, object>> include in includes.OrEmptyIfNull())
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}