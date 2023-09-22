using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Tardigrade.Framework.Patterns.DependencyInjection;

namespace Tardigrade.Framework.Testing
{
    /// <summary>
    /// An xUnit Class Fixture that supports User Secrets when running in a development environment.
    ///
    /// <a href="https://stackoverflow.com/questions/50921675/dependency-injection-in-xunit-project">Dependency injection in Xunit project</a>
    /// <a href="https://stackoverflow.com/questions/62537388/where-to-create-hostbuilder-and-avoid-the-following-constructor-parameters-did">Where to create HostBuilder and avoid 'The following constructor parameters did not have matching fixture data'</a>
    /// </summary>
    public abstract class UnitTestClassFixture : IDisposable, IServiceContainer
    {
        private const string DotNetEnvironment = "DOTNET_ENVIRONMENT";

        // Flag indicating if the current instance is already disposed.
        private bool _disposed;

        /// <summary>
        /// Specify the entry assembly for unit testing. This is used to determine the location of User Secrets. This
        /// property should be overwritten by a class fixture that exists in the assembly associated with the User
        /// Secrets. For testing, this property is generally set to Assembly.GetExecutingAssembly().
        /// </summary>
        protected virtual Assembly EntryAssembly => Assembly.GetEntryAssembly();

        /// <summary>
        /// Configured services for the unit tests.
        /// </summary>
        protected IServiceProvider Services => TestHost.Services;

        /// <summary>
        /// Unit test host.
        /// </summary>
        protected IHost TestHost { get; }

        /// <inheritdoc />
        ~UnitTestClassFixture() => Dispose(false);

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        protected UnitTestClassFixture()
        {
            // To enable the use of User Secrets, set the environment to development.
            // TODO Alternatively set by command shell instead, i.e. setx DOTNET_ENVIRONMENT "Development".
            Environment.SetEnvironmentVariable(DotNetEnvironment, "Development");

            // Create a host from which to load up configuration and register services.
            TestHost = CreateHostBuilder().Build();
            Task.Run(() => TestHost.RunAsync());
            UseServices(Services);
        }

        /// <summary>
        /// Add services to the service collection.
        /// </summary>
        /// <param name="context">Host builder context.</param>
        /// <param name="services">Services container.</param>
        protected abstract void ConfigureServices(HostBuilderContext context, IServiceCollection services);

        /// <summary>
        /// Create a host builder that correctly loads User Secrets for unit testing.
        /// </summary>
        /// <returns>Host builder.</returns>
        private IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    config.AddUserSecrets(EntryAssembly, true, true);
                }
            }).ConfigureServices(ConfigureServices);

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release all managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Should be false when called from a finalizer, and true when called from the IDisposable.Dispose method.</param>
        protected virtual void Dispose(bool disposing)
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

            Task.Run(() => TestHost.StopAsync());
            Environment.SetEnvironmentVariable(DotNetEnvironment, null);

            _disposed = true;
        }

        /// <inheritdoc />
        public T GetService<T>() where T : class
        {
            return Services.GetRequiredService<T>();
        }

        /// <summary>
        /// This method enables the injected services to be used to provision a test environment for the class fixture.
        /// </summary>
        /// <param name="serviceProvider">Service provider for accessing the services collection.</param>
        protected virtual void UseServices(IServiceProvider serviceProvider)
        {
        }
    }
}