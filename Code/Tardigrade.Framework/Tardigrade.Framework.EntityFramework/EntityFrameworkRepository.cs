using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Exceptions;
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
        public EntityFrameworkRepository(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Count()"/>
        /// </summary>
        public virtual int Count()
        {
            try
            {
                return DbContext.Set<T>().Count();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error calculating the number of objects of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
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
            catch (DbEntityValidationException e)
            {
                throw new ValidationException($"Error creating an object of type {typeof(T).Name} as it contains invalid values.", e);
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
        /// <see cref="ICrudRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, int?, int?, string[])"/>
        /// </summary>
        public virtual IList<T> Retrieve(Expression<Func<T, bool>> predicate = null, int? pageIndex = null, int? pageSize = null, string[] includes = null)
        {
            IList<T> objs = new List<T>();

            try
            {
                IQueryable<T> query = DbContext.Set<T>();

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                if ((pageIndex.HasValue && pageIndex.Value >= 0) && (pageSize.HasValue && pageSize.Value > 0))
                {
                    // https://visualstudiomagazine.com/articles/2016/12/06/skip-take-entity-framework-lambda.aspx
                    query = query.Skip(() => pageIndex.Value * pageSize.Value).Take(() => pageSize.Value);
                }

                if (includes != null)
                {
                    foreach (string include in includes)
                    {
                        query = query.Include(include);
                    }
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
                IDbSet<T> query = DbContext.Set<T>();

                if (includes != null)
                {
                    foreach (string include in includes)
                    {
                        query = (IDbSet<T>)query.Include(include);
                    }
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
        /// <see cref="ICrudRepository{T, PK}.Update(T)"/>
        /// </summary>
        public virtual void Update(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                DbContext.Set<T>().Attach(obj);
                DbContext.Entry(obj).State = EntityState.Modified;
                DbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw new ValidationException($"Error updating an object of type {typeof(T).Name} as it contains invalid values.", e);
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error updating an object of type {typeof(T).Name}.", e);
            }
        }
    }
}