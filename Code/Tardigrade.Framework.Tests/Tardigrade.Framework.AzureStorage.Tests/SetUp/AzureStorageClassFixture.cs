using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Tardigrade.Framework.AzureStorage.Configurations;
using Tardigrade.Framework.AzureStorage.Tables;
using Tardigrade.Framework.AzureStorage.Tests.Models;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Testing;

namespace Tardigrade.Framework.AzureStorage.Tests.SetUp;

/// <inheritdoc />
public class AzureStorageClassFixture : UnitTestClassFixture
{
    protected override Assembly EntryAssembly => Assembly.GetExecutingAssembly();

    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.Configure<IConfiguration>(context.Configuration);
        services.AddTransient<IAzureStorageConfiguration, AzureStorageConfiguration>();
        services.AddTransient<IRepository<FakeTableEntity, FakeTableKey>>(sp =>
            new Repository<FakeTableEntity, FakeTableKey>(
                sp.GetRequiredService<IAzureStorageConfiguration>().AzureStorageConnectionString, "MockTable"));
    }
}