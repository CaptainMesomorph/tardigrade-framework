using Microsoft.Extensions.DependencyInjection;

namespace Tardigrade.Framework.Patterns.DependencyInjection
{
    /// <summary>
    /// Service container based on Microsoft's Dependency Injection framework.
    /// </summary>
    public abstract class MicrosoftServiceContainer : IServiceContainer
    {
        /// <summary>
        /// Managed collection of services.
        /// </summary>
        protected IServiceCollection Services { get; private set; }

        /// <summary>
        /// Service provider for retrieving service instances.
        /// </summary>
        protected ServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Create an instance of this service container.
        /// </summary>
        public MicrosoftServiceContainer()
        {
            Services = new ServiceCollection();
            ConfigureServices(Services);
            ServiceProvider = Services.BuildServiceProvider();
        }

        /// <summary>
        /// <see cref="IServiceContainer.GetService{T}"/>
        /// </summary>
        public T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>();
        }

        /// <summary>
        /// Add services to the service container.
        /// </summary>
        /// <param name="services">Services container.</param>
        public abstract void ConfigureServices(IServiceCollection services);
    }
}