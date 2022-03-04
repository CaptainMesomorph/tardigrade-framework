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

        /// <summary>
        /// Specify the entry assembly for unit testing. This is used to determine the location of User Secrets.
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
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                if (hostingContext.HostingEnvironment.IsDevelopment())
                {
                    config.AddUserSecrets(EntryAssembly, true, true);
                }
            }).ConfigureServices(ConfigureServices);

        /// <inheritdoc />
        public void Dispose()
        {
            Task.Run(() => TestHost.StopAsync());
            Environment.SetEnvironmentVariable(DotNetEnvironment, null);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public T GetService<T>() where T : class
        {
            return Services.GetService<T>();
        }
    }
}