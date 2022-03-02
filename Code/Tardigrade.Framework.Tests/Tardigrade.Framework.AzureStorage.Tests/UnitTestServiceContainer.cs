using Microsoft.Extensions.DependencyInjection;
using Tardigrade.Framework.AzureStorage.Tables;
using Tardigrade.Framework.AzureStorage.Tests.Models;
using Tardigrade.Framework.Patterns.DependencyInjection;
using Tardigrade.Framework.Persistence;

namespace Tardigrade.Framework.AzureStorage.Tests;

internal class UnitTestServiceContainer : MicrosoftServiceContainer
{
    public override void ConfigureServices(IServiceCollection services)
    {
        var config = new ApplicationConfiguration();
        string connectionString = config.AzureStorageConnectionString;
        services.AddSingleton<IRepository<FakeTableEntity, FakeTableKey>>(
            new Repository<FakeTableEntity, FakeTableKey>(connectionString, "MockTable"));
    }
}