using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.IO;
using Tardigrade.Framework.Patterns.DependencyInjection;
using Tardigrade.Framework.Persistence;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models;
using Tardigrade.Shared.Tests.Models.Blogs;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/50921675/dependency-injection-in-xunit-project">Dependency injection in XUnit project</a>
    /// </summary>
    public class UnitTestFixture : IDisposable
    {
        private readonly IRepository<Blog, Guid> blogRepository;
        private readonly IRepository<User, Guid> userRepository;

        public IServiceContainer Container { get; }

        public Blog ReferenceBlog { get; }

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

            // Create and store SQL script for the test database.
            GenerateCreateScript(Container.GetService<DbContext>(), "Scripts/TestDataCreateScript.sql");

            // Create a reference blog for testing.
            ReferenceBlog = DataFactory.Blog;
            blogRepository = Container.GetService<IRepository<Blog, Guid>>();
            _ = blogRepository.Create(ReferenceBlog);

            // Create a reference user for testing.
            ReferenceUser = DataFactory.User;
            userRepository = Container.GetService<IRepository<User, Guid>>();
            _ = userRepository.Create(ReferenceUser);
        }

        public void Dispose()
        {
            // Delete the reference blog.
            if (blogRepository.Exists(ReferenceBlog.Id)) blogRepository.Delete(ReferenceBlog);

            // Delete the reference user.
            userRepository.Delete(ReferenceUser);
        }
    }
}