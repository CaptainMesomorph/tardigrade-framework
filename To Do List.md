# tardigrade-framework
Framework for supporting coding best practices.


## To do list

- Replace the deprecated Microsoft.Azure.Storage.Queue NuGet package. This package has been replaced by the following new Azure SDKs. You can read about the new Azure SDKs at https://aka.ms/azsdkvalueprop. The latest libraries to interact with the Azure Storage service are:
  - https://www.nuget.org/packages/Azure.Storage.Blobs
  - https://www.nuget.org/packages/Azure.Storage.Queues/
  - https://www.nuget.org/packages/Azure.Storage.Files.Shares/
- Move the IdentityUserDbContext class from the EntityFramework project to the AspNet project.
- Move the IdentityUserDbContext class from the EntityFrameworkCore project to the AspNetCore project.
