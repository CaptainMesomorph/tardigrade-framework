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
        /// <see cref="IRepository{T, PK}.Create(T, IUnitOfWork)"/>
        /// </summary>
        public virtual T Create(T obj, IUnitOfWork unitOfWork = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            try
            {
                DbContext dbContext = (unitOfWork as EntityFrameworkUnitOfWork)?.DbContext ?? DbContext;
                dbContext.Set<T>().Add(obj);
                dbContext.SaveChanges();
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
        /// <see cref="IRepository{T, PK}.Delete(PK, IUnitOfWork)"/>
        /// </summary>
        public virtual void Delete(PK id, IUnitOfWork unitOfWork = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            try
            {
                DbContext dbContext = (unitOfWork as EntityFrameworkUnitOfWork)?.DbContext ?? DbContext;
                T obj = dbContext.Set<T>().Find(id);
                dbContext.Set<T>().Remove(obj);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name} with unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Delete(T, IUnitOfWork)"/>
        /// </summary>
        public virtual void Delete(T obj, IUnitOfWork unitOfWork = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            try
            {
                DbContext dbContext = (unitOfWork as EntityFrameworkUnitOfWork)?.DbContext ?? DbContext;

                if (dbContext.Entry(obj).State == EntityState.Detached)
                {
                    dbContext.Set<T>().Attach(obj);
                }

                dbContext.Set<T>().Remove(obj);
                dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new RepositoryException($"Error deleting an object of type {typeof(T).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IRepository{T, PK}.Retrieve(Expression{Func{T, bool}}, int?, int?, string[])"/>
        /// </summary>
        public virtual IList<T> Retrieve(Expression<Func<T, bool>> predicate = null, int? pageIndex = null, int? pageSize = null, string[] includes = null)
        {
            IList<T> objs = new List<T>();

            try
            {
                if (includes == null)
                {
                    DbContext.Configuration.ProxyCreationEnabled = true;
                }
                else
                {
                    DbContext.Configuration.ProxyCreationEnabled = false;
                }

                IQueryable<T> query = DbContext.Set<T>();

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                if ((pageIndex.HasValue && pageIndex.Value >= 0) && (pageSize.HasValue && pageSize.Value > 0))
                {
                    query = query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);
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
        /// <see cref="IRepository{T, PK}.Retrieve(PK, string[])"/>
        /// </summary>
        public virtual T Retrieve(PK id, string[] includes = null)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            T obj = default(T);

            try
            {
                if (includes == null)
                {
                    DbContext.Configuration.ProxyCreationEnabled = true;
                }
                else
                {
                    DbContext.Configuration.ProxyCreationEnabled = false;
                }

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
        /// <see cref="IRepository{T, PK}.Update(T, IUnitOfWork)"/>
        /// </summary>
        public virtual void Update(T obj, IUnitOfWork unitOfWork = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            try
            {
                DbContext dbContext = (unitOfWork as EntityFrameworkUnitOfWork)?.DbContext ?? DbContext;
                dbContext.Set<T>().Attach(obj);
                dbContext.Entry(obj).State = EntityState.Modified;
                dbContext.SaveChanges();
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