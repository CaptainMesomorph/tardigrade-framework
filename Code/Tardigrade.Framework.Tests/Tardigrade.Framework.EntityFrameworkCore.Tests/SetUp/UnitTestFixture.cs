using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.IO;
using Tardigrade.Framework.Patterns.DependencyInjection;
using Tardigrade.Framework.Persistence;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/50921675/dependency-injection-in-xunit-project">Dependency injection in XUnit project</a>
    /// </summary>
    public class UnitTestFixture : IDisposable
    {
        private readonly IRepository<User, Guid> repository;

        public IServiceContainer Container { get; }

        public User ReferenceUser { get; }

        /// <summary>
        /// Generate SQL script for the creation of the database tables required for unit testing.
        /// <a href="https://stackoverflow.com/questions/38532764/create-tables-on-runtime-in-ef-core">Create Tables on runtime in EF Core</a>
        /// <a href="https://stackoverflow.com/questions/816566/how-do-you-get-the-current-project-directory-from-c-sharp-code-when-creating-a-c">How do you get the current project directory from C# code when creating a custom MSBuild task?</a>
        /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-write-text-to-a-file">How to: Write text to a file</a>
        /// </summary>
        /// <param name="dbContext">Database context.</param>
        /// <param name="scriptFilename">Name (and path) of the script file to generate.</param>
        private static void GenerateCreateScript(DbContext dbContext, string scriptFilename)
        {
            // Generate SQL script for the database schema.
            dbContext.Database.EnsureCreated();
            var databaseCreator = (RelationalDatabaseCreator)dbContext.Database.GetService<IDatabaseCreator>();
            string createScript = databaseCreator.GenerateCreateScript();

            // Get the current project's directory to store the create script.
            DirectoryInfo binDirectory =
                Directory.GetParent(Directory.GetCurrentDirectory()).Parent ??
                Directory.GetParent(Directory.GetCurrentDirectory());
            DirectoryInfo projectDirectory = binDirectory.Parent ?? binDirectory;

            // Save the create script.
            using var outputFile = new StreamWriter(Path.Combine(projectDirectory.FullName, scriptFilename));
            outputFile.WriteLine(createScript);
        }

        public UnitTestFixture()
        {
            Container = new UnitTestServiceContainer();

            // Create a reference user for testing.
            ReferenceUser = DataFactory.CreateUser();
            repository = Container.GetService<IRepository<User, Guid>>();
            _ = repository.Create(ReferenceUser);

            // Create and store SQL script for the test database.
            GenerateCreateScript(Container.GetService<DbContext>(), "Scripts/TestDataCreateScript.sql");
        }

        public void Dispose()
        {
            // Delete the reference user.
            repository.Delete(ReferenceUser);
        }
    }
}