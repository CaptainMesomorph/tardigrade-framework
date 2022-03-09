using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.IO;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Tenants;

namespace Tardigrade.Framework.EntityFrameworkCore.Extensions;

/// <summary>
/// This static class contains extension methods for the DbContext class.
/// </summary>
public static class DbContextExtension
{
    private static readonly object Lock = new();

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
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    case EntityState.Modified:
                    default:
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

    /// <summary>
    /// Generate an SQL create script for the database tables associated with the database context.
    /// <a href="https://stackoverflow.com/questions/38532764/create-tables-on-runtime-in-ef-core">Create Tables on runtime in EF Core</a>
    /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-write-text-to-a-file">How to: Write text to a file</a>
    /// </summary>
    /// <param name="dbContext">Database context.</param>
    /// <param name="scriptFilename">Name (and path) of the script file to generate.</param>
    /// <param name="replaceExisting">If true, replace existing script file; if false, leave existing script file as is.</param>
    /// <exception cref="ArgumentNullException">dbContext is null, or scriptFilename is null or empty.</exception>
    public static void GenerateCreateScript(
        this DbContext dbContext,
        string scriptFilename,
        bool replaceExisting = false)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        if (string.IsNullOrWhiteSpace(scriptFilename)) throw new ArgumentNullException(nameof(scriptFilename));

        if (File.Exists(scriptFilename) && !replaceExisting) return;

        // Generate SQL script for the database schema.
        dbContext.Database.EnsureCreated();
        var databaseCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();
        string createScript = databaseCreator.GenerateCreateScript();

        lock (Lock)
        {
            // Save the create script.
            using var outputFile = new StreamWriter(scriptFilename);
            outputFile.WriteLine(createScript);
        }
    }
}