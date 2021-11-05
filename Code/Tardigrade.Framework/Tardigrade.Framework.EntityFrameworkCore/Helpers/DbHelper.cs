using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.IO;

namespace Tardigrade.Framework.EntityFrameworkCore.Helpers
{
    /// <summary>
    /// Helper class for database related operations.
    /// </summary>
    public static class DbHelper
    {
        private static readonly object Lock = new();

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
            DbContext dbContext,
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
}