# tardigrade-framework
Framework for supporting coding best practices.


## Version control history

**June 21, 2019 - 6.1.0 Enhanced the ApplicationUserManager**

- Extended the ApplicationUserManager to include additional operations.
- Updated the ObjectController to reference asynchronous service methods instead of synchronous.

**June 20, 2019 - 6.0.0 Enhanced the service layer to include async methods**

- Created application user interfaces to "wrap" ASP.NET and ASP.NET Core Identity services.
- Created new ASP.NET and ASP.NET Core specific projects for the application user implementations.
- Created a service for managing JSON Web Tokens.
- Created a common object definition to standardise API responses.
- Created a generic ObjectController based upon ASP.NET Core.
- Converted the Tardigrade.Framework.EntityFramework project from .NET Standard to .NET Framework.
- Converted the Tardigrade.Framework.EntityFrameworkCore project from .NET Standard to .NET Core.
- Merged the Tardigrade.Framework.Serialization project into Tardigrade.Framework and then removed it.
- Added a method for bulk creation in the repository layer.
- Extended the service layer to add corresponding asynchronous methods.


**June 13, 2019 - 5.0.0 Enhanced the repository layer to include async methods**

- Enhanced the repository layer to incorporate asynchronous equivalents to existing methods.
- Updated the repository interface to include NotFound (when deleting or updating) and AlreadyExists (when creating) exceptions.
- Created an xUnit test project for the AzureStorage project.
- Created a basic interface to manage Dependency Injection frameworks from Microsoft and SimpleInjector.
- Cleaned up Project (csproj) files and resolved XML documentation generation issues.
- Updated project licences.

**May 28, 2019 - 4.0.0 Created a DTO service and enhanced the repository layer**

- Created a Data Transfer Object (DTO) service.
- Merged the ICrudRepository interface into IRepository and removed it to simplify usage.
- Configured the generation of XML documentation on build.

**May 7, 2019 - 3.1.0 Resolved an EF/LINQ issue with the repository layer**

- Updated the EntityFramework 6 and EntityFramework Core projects to resolve an EF/LINQ issue with casting from a uint calculation result to an int.

**Apr 15, 2019 - 3.0.0 Enhanced the paging implementation of the repository layer**

- Enhanced the paging implementation of the repository layer.
- Created a project for managing (JSON) serialisation.
- Added new extention classes to simplify coding.

**Nov 30, 2018 - 2.0.0 Updated the design of the repository interfaces**

- Updated the design of the repository interfaces.

**Nov 26, 2018 - 1.0.1 Updated referenced NuGet packages**

- Updated the WindowsAzure.Storage and RestSharp NuGet packages.

**Nov 25, 2018 - 1.0.0 Initial submission of the framework**

- Initial release of the framework including integrations for EntityFramework 6, EntityFramework Core, Azure Storage Tables, RestSharp and SimpleInjector.
