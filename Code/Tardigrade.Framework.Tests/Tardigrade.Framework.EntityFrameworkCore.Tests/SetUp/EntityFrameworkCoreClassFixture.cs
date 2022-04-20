using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Tardigrade.Framework.EntityFrameworkCore.Tests.Data;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Testing;
using Tardigrade.Shared.Tests;
using Tardigrade.Shared.Tests.Models.Blogs;
using Tardigrade.Shared.Tests.Models.Credentials;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp;

/// <inheritdoc />
public class EntityFrameworkCoreClassFixture : UnitTestClassFixture
{
    private IRepository<Blog, Guid>? _blogRepository;
    private IRepository<Person, Guid>? _personRepository;
    private IRepository<User, Guid>? _userRepository;

    // Flag indicating if the current instance is already disposed.
    private bool _disposed;

    protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

    public Blog ReferenceBlog { get; private set; } = new();

    public Person ReferencePerson { get; private set; } = new();

    public User ReferenceUser { get; private set; } = new();

    /// <inheritdoc />
    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        // For self-hosted unit testing, services.AddDbContext() did not provide an accessible DbContext.
        string connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        DbContextOptions<TestDataDbContext> options =
            new DbContextOptionsBuilder<TestDataDbContext>().UseSqlite(connectionString).Options;
        services.AddTransient<DbContext>(_ => new TestDataDbContext(options));

        // Inject business services.
        services.AddTransient<IRepository<Blog, Guid>, Repository<Blog, Guid>>();
        services.AddTransient<IRepository<Person, Guid>, Repository<Person, Guid>>();
        services.AddTransient<IRepository<Post, Guid>, Repository<Post, Guid>>();
        services.AddTransient<IRepository<User, Guid>, Repository<User, Guid>>();
        services.AddTransient<IRepository<UserCredential, Guid>, Repository<UserCredential, Guid>>();
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
            // Dispose managed state (managed objects).
        }

        // Free unmanaged resources (unmanaged objects) and override finalizer.
        // Set large fields to null.

        if (_blogRepository?.Exists(ReferenceBlog.Id) ?? false) _blogRepository.Delete(ReferenceBlog);
        if (_personRepository?.Exists(ReferencePerson.Id) ?? false) _personRepository.Delete(ReferencePerson);
        if (_userRepository?.Exists(ReferenceUser.Id) ?? false) _userRepository.Delete(ReferenceUser);

        _disposed = true;

        base.Dispose(disposing);
    }

    public void PopulateDataStore()
    {
        try
        {
            // Create a reference Blog for testing.
            ReferenceBlog = DataFactory.Blog;
            ReferenceBlog.Posts = DataFactory.Posts;
            _blogRepository = GetService<IRepository<Blog, Guid>>();
            _ = _blogRepository.Create(ReferenceBlog);

            // Create a reference Person for testing.
            ReferencePerson = DataFactory.CreatePerson();
            _personRepository = GetService<IRepository<Person, Guid>>();
            _ = _personRepository.Create(ReferencePerson);

            // Create a reference User for testing.
            ReferenceUser = DataFactory.User;
            _userRepository = GetService<IRepository<User, Guid>>();
            _ = _userRepository.Create(ReferenceUser);
        }
        catch
        {
            if (_blogRepository?.Exists(ReferenceBlog.Id) ?? false) _blogRepository.Delete(ReferenceBlog);
            if (_personRepository?.Exists(ReferencePerson.Id) ?? false) _personRepository.Delete(ReferencePerson);
            if (_userRepository?.Exists(ReferenceUser.Id) ?? false) _userRepository.Delete(ReferenceUser);
        }
    }
}