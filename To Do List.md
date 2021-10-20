# tardigrade-framework
Framework for supporting coding best practices.


## To do list

- Update .NET Framework 4.7.2 projects to .NET Framework 4.8.
- Fix broken unit tests.
- Recreate the Tardigrade.Framework.EntityFramework.Tests project as a .NET Standard project.
- Update Tardigrade.Framework.EntityFrameworkCore.Repository<TEntity, TKey>.UpdateAsync(TEntity, CancellationToken) to cater for DbUpdateConcurrencyException.
- Update Tardigrade.Framework.Persistence.IReadOnlyRepository<TEntity, in TKey> to include Exists methods that use a filter condition, i.e. Exists(Expression<Func<TEntity, bool>>) and CountAsync(Expression<Func<TEntity, bool>>, CancellationToken).
- For security reasons, handle all unexpected exceptions raised from the repository to return RepositoryException. This will help prevent table names being exposed if details of the base exception are not automaticaly referenced by calling code.
- Replace the deprecated Microsoft.Azure.Storage.Queue NuGet package. This package has been replaced by the following new Azure SDKs. You can read about the new Azure SDKs at https://aka.ms/azsdkvalueprop. The latest libraries to interact with the Azure Storage service are:
  - https://www.nuget.org/packages/Azure.Storage.Blobs
  - https://www.nuget.org/packages/Azure.Storage.Queues/
  - https://www.nuget.org/packages/Azure.Storage.Files.Shares/
- Create a new Tardigrade.Framework.IdentityAspNet project. Move any identity-related classes to this project from the AspNet project.
- Create a new Tardigrade.Framework.IdentityAspNetCore project. Move any identity-related classes to this project from the AspNetCore project and the EntityFrameworkCore project (including the IdentityUserDbContext class).
- Create a Configuration Provider that reads configuration key-value pairs from a database using Entity Framework (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#custom-configuration-provider).
