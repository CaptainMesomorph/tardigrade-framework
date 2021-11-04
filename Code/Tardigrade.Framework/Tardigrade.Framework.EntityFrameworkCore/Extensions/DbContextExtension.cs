using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Tenants;

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
        /// <param name="dbContext">Database context.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>
        public static void ApplySoftDeletion(this DbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

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
                            // TODO Would the Modified state be more appropriate?
                            entry.State = EntityState.Unchanged;
                            entity.IsDeleted = true;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Configure entity states to cater for tenants.
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        /// <param name="tenant">Tenant to apply.</param>
        /// <exception cref="ArgumentNullException">dbContext is null.</exception>
        public static void ApplyTenant(this DbContext dbContext, string tenant)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            if (string.IsNullOrWhiteSpace(tenant)) throw new ArgumentNullException(nameof(tenant));

            foreach (EntityEntry entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is IHasTenant<string> entity)
                {
                    entity.Tenant = entry.State switch
                    {
                        EntityState.Added => tenant,
                        _ => entity.Tenant
                    };
                }
            }
        }
    }
}