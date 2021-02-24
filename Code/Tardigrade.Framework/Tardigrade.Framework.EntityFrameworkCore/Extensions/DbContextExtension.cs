using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tardigrade.Framework.Models.Domain;

namespace Tardigrade.Framework.EntityFrameworkCore.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the DbContext class.
    /// </summary>
    public static class DbContextExtension
    {
        /// <summary>
        /// Configure entity states to apply soft deletion.
        /// </summary>
        public static void ApplySoftDeletion(this DbContext dbContext)
        {
            foreach (EntityEntry entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is ISoftDeletable entity)
                {
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            entity.IsDeleted = false;
                            break;

                        case EntityState.Deleted:
                            //entry.State = EntityState.Modified;
                            entry.State = EntityState.Unchanged;
                            entity.IsDeleted = true;
                            break;
                    }
                }
            }
        }
    }
}