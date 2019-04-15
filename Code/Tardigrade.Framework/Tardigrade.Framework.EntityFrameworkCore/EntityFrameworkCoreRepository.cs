using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFrameworkCore
{
    /// <summary>
    /// <see cref="IRepository{T, PK}"/>
    /// </summary>
    public class EntityFrameworkCoreRepository<T, PK> : IRepository<T, PK> where T : class
    {
        /// <summary>
        /// Database context.
        /// </summary>
        protected DbContext DbContext { get; private set; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        public EntityFrameworkCoreRepository(DbContext dbContext)
        {
            DbContext = dbContext;
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
            catch (Exception e)
            {
                throw new RepositoryException($"Error calculating the number of objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// ValidationException currently not supported.
        /// <see cref="ICrudRepository{T, PK}.Create(T)"/>
        /// </summary>
        public virtual T Create(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                DbContext.Set<T>().Add(obj);
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error creating an object of type {typeof(T).Name}.", e);
            }

            return obj;
        }

        /// <summary>
        /// <see cref="ICrudRepository{T, PK}.Delete(PK)"/>
        /// </summary>
        public virtual void Delete(PK id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            try
            {
                T obj = DbContext.Set<T>().Find(id);
                DbContext.Set<T>().Remove(obj);
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} with unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="ICrudRepository{T, PK}.Delete(T)"/>
        /// </summary>
        public virtual void Delete(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                if (DbContext.Entry(obj).State == EntityState.Detached)
                {
                    DbContext.Set<T>().Attach(obj);
                }

                DbContext.Set<T>().Remove(obj);
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name}.", e);
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
                return DbContext.Set<T>().Find(id) != null;
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error determing whether an object of type {typeof(T).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="ICrudRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<T> Retrieve(
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

            try
            {
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
                        query = query
                            .Skip((int)(pagingContext.PageIndex * pagingContext.PageSize))
                            .Take((int)pagingContext.PageSize);
                    }
                }

                foreach (Expression<Func<T, object>> include in includes.OrEmptyIfNull())
                {
                    query = query.Include(include);
                }

                objs = query.ToList();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error retrieving objects of type {typeof(T).Name}.", e);
            }

            return objs;
        }

        /// <summary>
        /// <see cref="ICrudRepository{T, PK}.Retrieve(PK, string[])"/>
        /// </summary>
        public virtual T Retrieve(PK id, string[] includes = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            T obj = default(T);

            try
            {
                DbSet<T> query = DbContext.Set<T>();

                foreach (string include in includes.OrEmptyIfNull())
                {
                    query = (DbSet<T>)query.Include(include);
                }

                obj = query.Find(id);
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error retrieving an object of type {typeof(T).Name} with unique identifier of {id}.", e);
            }

            return obj;
        }

        /// <summary>
        /// ValidationException currently not supported.
        /// <see cref="ICrudRepository{T, PK}.Update(T)"/>
        /// </summary>
        public virtual void Update(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            try
            {
                DbContext.Set<T>().Update(obj);
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error updating an object of type {typeof(T).Name}.", e);
            }
        }
    }
}