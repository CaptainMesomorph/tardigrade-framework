using System;
using System.Data.Entity;
using Tardigrade.Framework.Patterns.UnitOfWork;

namespace Tardigrade.Framework.EntityFramework
{
    /// <summary>
    /// <see cref="IUnitOfWork"/>
    /// </summary>
    public sealed class EntityFrameworkUnitOfWork : IUnitOfWork, IDisposable
    {
        private DbContextTransaction transaction;
        private int count = 0;

        /// <summary>
        /// Database context.
        /// </summary>
        public DbContext DbContext { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="dbContext">Database context used to define a Unit of Work.</param>
        public EntityFrameworkUnitOfWork(DbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        /// <summary>
        /// <see cref="IUnitOfWork.Begin()"/>
        /// </summary>
        public void Begin()
        {
            if (count == 0)
            {
                transaction = DbContext.Database.BeginTransaction();
            }

            count++;
        }

        /// <summary>
        /// <see cref="IUnitOfWork.Commit()"/>
        /// </summary>
        public void Commit()
        {
            count--;

            if (count == 0)
            {
                transaction.Commit();
            }
        }

        /// <summary>
        /// <see cref="IDisposable.Dispose()"/>
        /// </summary>
        public void Dispose()
        {
            DbContext?.Dispose();
        }

        /// <summary>
        /// <see cref="IUnitOfWork.Rollback()"/>
        /// </summary>
        public void Rollback()
        {
            count--;

            if (count == 0)
            {
                transaction.Rollback();
            }
        }
    }
}