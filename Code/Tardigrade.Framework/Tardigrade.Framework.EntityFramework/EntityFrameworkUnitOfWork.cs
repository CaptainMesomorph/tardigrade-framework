using System;
using System.Data.Entity;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.EntityFramework
{
    /// <summary>
    /// <see cref="IUnitOfWork"/>
    /// </summary>
    public sealed class EntityFrameworkUnitOfWork : IUnitOfWork, IDisposable
    {
        private DbContextTransaction transaction;
        private int count = 0;

        public DbContext DbContext { get; private set; }

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
            if (DbContext != null)
            {
                DbContext.Dispose();
            }
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