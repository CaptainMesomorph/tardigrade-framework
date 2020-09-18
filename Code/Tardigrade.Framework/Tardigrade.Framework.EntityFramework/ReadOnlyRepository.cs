using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.EntityFramework.Extensions;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFramework
{
    /// <summary>
    /// <see cref="IReadOnlyRepository{T, PK}"/>
    /// </summary>
    public class ReadOnlyRepository<TEntity, TKey> : ReadOnlyRepository<TEntity, TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>
        /// <exception cref="RepositoryException">Object type TEntity are not recognised in the dbContext.</exception>
        public ReadOnlyRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }

    /// <summary>
    /// Read-only repository for a derived type from a type inheritance hierarchy.
    /// <see cref="IReadOnlyRepository{T, PK}"/>
    /// </summary>
    /// <typeparam name="TBaseEntity">Base type associated with the repository operations.</typeparam>
    /// <typeparam name="TDerivedEntity">Derived type associated with the repository operations.</typeparam>
    /// <typeparam name="TKey">Unique identifier type for the object type.</typeparam>
    public class ReadOnlyRepository<TBaseEntity, TDerivedEntity, TKey> : IReadOnlyRepository<TDerivedEntity, TKey>
        where TBaseEntity : class
        where TDerivedEntity : class, TBaseEntity
    {
        /// <summary>
        /// For a base class, this set defines the collection of base entities in the database context.
        /// For a derived class, this set defines the collection of base entities (that the derived class inherits
        /// from) in the database context.
        /// </summary>
        protected DbSet<TBaseEntity> BaseEntityDbSet { get; set; }

        /// <summary>
        /// Database context.
        /// </summary>
        protected DbContext DbContext { get; }

        /// <summary>
        /// For a base class, this query function applies to the collection of base entities.
        /// For a derived class, this query function applies to the collection of derived entities that inherit from
        /// the base class.
        /// </summary>
        protected IQueryable<TDerivedEntity> DerivedEntityQueryable;

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>
        /// <exception cref="RepositoryException">Object type TBaseEntity or TDerivedEntity are not recognised in the dbContext.</exception>
        public ReadOnlyRepository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            if (DbContext.Set<TBaseEntity>() == null)
            {
                throw new RepositoryException(
                    $"Base type {typeof(TBaseEntity).Name} is not recognised in the DbContext.");
            }

            if (DbContext.Set<TBaseEntity>().OfTypeOnly<TBaseEntity, TDerivedEntity>() == null)
            {
                throw new RepositoryException(
                    $"Derived type {typeof(TDerivedEntity).Name} is not recognised in the DbContext.");
            }

            BaseEntityDbSet = DbContext.Set<TBaseEntity>();
            DerivedEntityQueryable = DbContext.Set<TBaseEntity>().OfTypeOnly<TBaseEntity, TDerivedEntity>();
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        public virtual int Count(Expression<Func<TDerivedEntity, bool>> filter = null)
        {
            try
            {
                return filter == null ? DerivedEntityQueryable.Count() : DerivedEntityQueryable.Count(filter);
            }
            catch (OverflowException e)
            {
                throw new RepositoryException(
                    $"Count failed; more than {int.MaxValue} objects of type {typeof(TDerivedEntity).Name} exist.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<TDerivedEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return filter == null
                    ? await DerivedEntityQueryable.CountAsync(cancellationToken)
                    : await DerivedEntityQueryable.CountAsync(filter, cancellationToken);
            }
            catch (OverflowException e)
            {
                throw new RepositoryException(
                    $"Count failed; more than {int.MaxValue} objects of type {typeof(TDerivedEntity).Name} exist.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.Exists(PK)"/>
        /// </summary>
        public virtual bool Exists(TKey id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            try
            {
                return BaseEntityDbSet.Find(id) != null;
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Exists failed; unable to determine whether object of type {typeof(TDerivedEntity).Name} with primary key {id} exists.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.ExistsAsync(PK, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(
            TKey id,
            CancellationToken cancellationToken = default)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            var item = (TDerivedEntity)await BaseEntityDbSet.FindAsync(cancellationToken, id);

            return item != null;
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<TDerivedEntity> Retrieve(
            Expression<Func<TDerivedEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TDerivedEntity>, IOrderedQueryable<TDerivedEntity>> sortCondition = null,
            params Expression<Func<TDerivedEntity, object>>[] includes)
        {
            return RetrieveQuery(filter, pagingContext, sortCondition, includes).ToList();
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
        /// <a href="https://docs.microsoft.com/en-au/ef/ef6/querying/related-data">Loading Related Entities</a>
        /// <a href="https://devblogs.microsoft.com/csharpfaq/how-can-i-get-objects-and-property-values-from-expression-trees/">How can I get objects and property values from expression trees?</a>
        /// </summary>
        public virtual TDerivedEntity Retrieve(TKey id, params Expression<Func<TDerivedEntity, object>>[] includes)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            try
            {
                var item = (TDerivedEntity)BaseEntityDbSet.Find(id);

                if (item == null)
                {
                    return null;
                }

                DbEntityEntry<TDerivedEntity> entity = DbContext.Entry(item);

                foreach (Expression<Func<TDerivedEntity, object>> include in includes)
                {
                    try
                    {
                        entity.Reference(include).Load();
                    }
                    catch (ArgumentException)
                    {
                        var expression = (MemberExpression)include.Body;
                        string name = expression.Member.Name;

                        try
                        {
                            entity.Collection(name).Load();
                        }
                        catch (ArgumentException e)
                        {
                            throw new RepositoryException(
                                $"Retrieve failed; a lazy-loading error occurred retrieving object of type {typeof(TDerivedEntity).Name} with primary key {id}.",
                                e);
                        }
                    }
                }

                return item;
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Retrieve failed; database error while retrieving object of type {typeof(TDerivedEntity).Name} with primary key {id}.",
                    e);
            }
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual async Task<IEnumerable<TDerivedEntity>> RetrieveAsync(
            Expression<Func<TDerivedEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TDerivedEntity>, IOrderedQueryable<TDerivedEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TDerivedEntity, object>>[] includes)
        {
            return await RetrieveQuery(filter, pagingContext, sortCondition, includes).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// <see cref="IReadOnlyRepository{T, PK}.RetrieveAsync(PK, CancellationToken, Expression{Func{T, object}}[])"/>
        /// <a href="https://stackoverflow.com/questions/39434878/how-to-include-related-tables-in-dbset-find">How to include related tables in DbSet.Find()?</a>
        /// <a href="https://docs.microsoft.com/en-au/ef/ef6/querying/related-data">Loading Related Entities</a>
        /// <a href="https://devblogs.microsoft.com/csharpfaq/how-can-i-get-objects-and-property-values-from-expression-trees/">How can I get objects and property values from expression trees?</a>
        /// </summary>
        public virtual async Task<TDerivedEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TDerivedEntity, object>>[] includes)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));

            try
            {
                var item = (TDerivedEntity)await BaseEntityDbSet.FindAsync(cancellationToken, id);

                if (item == null)
                {
                    return null;
                }

                DbEntityEntry<TDerivedEntity> entity = DbContext.Entry(item);

                foreach (Expression<Func<TDerivedEntity, object>> include in includes)
                {
                    try
                    {
                        await entity.Reference(include).LoadAsync(cancellationToken);
                    }
                    catch (ArgumentException)
                    {
                        var expression = (MemberExpression)include.Body;
                        string name = expression.Member.Name;

                        try
                        {
                            await entity.Collection(name).LoadAsync(cancellationToken);
                        }
                        catch (ArgumentException e)
                        {
                            throw new RepositoryException(
                                $"Retrieve failed; a lazy-loading error occurred retrieving object of type {typeof(TDerivedEntity).Name} with primary key {id}.",
                                e);
                        }
                    }
                }

                return item;
            }
            catch (InvalidOperationException e)
            {
                throw new RepositoryException(
                    $"Retrieve failed; database error while retrieving object of type {typeof(TDerivedEntity).Name} with primary key {id}.",
                    e);
            }
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
        private IQueryable<TDerivedEntity> RetrieveQuery(
            Expression<Func<TDerivedEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TDerivedEntity>, IOrderedQueryable<TDerivedEntity>> sortCondition = null,
            params Expression<Func<TDerivedEntity, object>>[] includes)
        {
            if (pagingContext != null && sortCondition == null)
            {
                throw new ArgumentException(
                    $"{nameof(sortCondition)} is required if {nameof(pagingContext)} is provided.");
            }

            IQueryable<TDerivedEntity> query = DerivedEntityQueryable;

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
                    var skip = (int)(pagingContext.PageIndex * pagingContext.PageSize);
                    // https://visualstudiomagazine.com/articles/2016/12/06/skip-take-entity-framework-lambda.aspx
                    query = query
                        .Skip(() => skip)
                        .Take(() => (int)pagingContext.PageSize);
                }
            }

            foreach (Expression<Func<TDerivedEntity, object>> include in includes.OrEmptyIfNull())
            {
                query = query.Include(include);
            }

            return query;
        }
    }
}