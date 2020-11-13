# tardigrade-framework
Framework for supporting coding best practices.


## To do list

- Replace the deprecated Microsoft.Azure.Storage.Queue NuGet package. This package has been replaced by the following new Azure SDKs. You can read about the new Azure SDKs at https://aka.ms/azsdkvalueprop. The latest libraries to interact with the Azure Storage service are:
  - https://www.nuget.org/packages/Azure.Storage.Blobs
  - https://www.nuget.org/packages/Azure.Storage.Queues/
  - https://www.nuget.org/packages/Azure.Storage.Files.Shares/
- Create a new Tardigrade.Framework.IdentityAspNet project. Move any identity-related classes to this project from the AspNet project.
- Create a new Tardigrade.Framework.IdentityAspNetCore project. Move any identity-related classes to this project from the AspNetCore project and the EntityFrameworkCore project (including the IdentityUserDbContext class).
- Create a Configuration Provider that reads configuration key-value pairs from a database using Entity Framework (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#custom-configuration-provider).
