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
using Tardigrade.Shared.Tests.Models.Credentials;

namespace Tardigrade.Framework.EntityFrameworkCore.Tests.SetUp;

/// <inheritdoc />
public class UserClassFixture : UnitTestClassFixture
{
    private IRepository<User, Guid>? _userRepository;

    // Flag indicating if the current instance is already disposed.
    private bool _disposed;

    protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

    public User ReferenceUser { get; } = DataFactory.User;

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
            if (_userRepository?.Exists(ReferenceUser.Id) ?? false)
            {
                _userRepository.Delete(ReferenceUser);
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
        // Create a reference User for testing.
        _userRepository = serviceProvider.GetRequiredService<IRepository<User, Guid>>();
        _ = _userRepository.Create(ReferenceUser);
    }
}