using System;
using Tardigrade.Framework.Patterns.DependencyInjection;

namespace Tardigrade.Framework.AzureStorage.Tests
{
    /// <summary>
    /// <a href="https://stackoverflow.com/questions/50921675/dependency-injection-in-xunit-project">Dependency injection in Xunit project</a>
    /// </summary>
    public class ServiceProviderFixture : IDisposable
    {
        public IServiceContainer Container { get; private set; }

        public ServiceProviderFixture()
        {
            Container = new UnitTestServiceContainer();
        }

        public void Dispose()
        {
        }
    }
}