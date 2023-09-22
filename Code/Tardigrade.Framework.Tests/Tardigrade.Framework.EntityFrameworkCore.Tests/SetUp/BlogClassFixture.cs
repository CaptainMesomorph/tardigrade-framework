using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
using System.Reflection;
using Tardigrade.Framework.EntityFrameworkCore.Tests.Data;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Testing;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models.Blogs;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp;

/// <inheritdoc />
public class BlogClassFixture : UnitTestClassFixture
{
    private IRepository<Blog, Guid>? _blogRepository;
    private IRepository<Person, Guid>? _personRepository;

    // Flag indicating if the current instance is already disposed.
    private bool _disposed;

    protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

    public Blog ReferenceBlog { get; } = DataFactory.Blog;

    public Person ReferencePerson { get; } = DataFactory.CreatePerson();

    /// <inheritdoc />
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // For self-hosted unit testing, services.AddDbContext() did not provide an accessible DbContext.
        string connectionString = context.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new ConfigurationErrorsException("Database connection string not defined.");
        DbContextOptions<TestDataDbContext> options =
            new DbContextOptionsBuilder<TestDataDbContext>().UseSqlite(connectionString).Options;
        services.AddTransient<DbContext>(_ => new TestDataDbContext(options));

        // Inject business services.
        services.AddTransient<IRepository<Blog, Guid>, Repository<Blog, Guid>>();
        services.AddTransient<IRepository<Person, Guid>, Repository<Person, Guid>>();
        services.AddTransient<IRepository<Post, Guid>, Repository<Post, Guid>>();
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            if (_blogRepository?.Exists(ReferenceBlog.Id) ?? false)
            {
                _blogRepository.Delete(ReferenceBlog);
            }

            if (_personRepository?.Exists(ReferencePerson.Id) ?? false)
            {
                _personRepository.Delete(ReferencePerson);
            }
        }

        // Free unmanaged resources (unmanaged objects) and override finalizer.
        // Set large fields to null.

        _disposed = true;

        base.Dispose(disposing);
    }

    /// <inheritdoc />
    protected override void UseServices(IServiceProvider serviceProvider)
    {
        // Create a reference Blog for testing.
        ReferenceBlog.Posts = DataFactory.Posts;
        _blogRepository = GetService<IRepository<Blog, Guid>>();
        _ = _blogRepository.Create(ReferenceBlog);

        try
        {
            // Create a reference Person for testing.
            _personRepository = GetService<IRepository<Person, Guid>>();
            _ = _personRepository.Create(ReferencePerson);
        }
        catch
        {
            if (_blogRepository?.Exists(ReferenceBlog.Id) ?? false)
            {
                _blogRepository.Delete(ReferenceBlog);
            }
        }
    }
}