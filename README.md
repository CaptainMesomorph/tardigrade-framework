# tardigrade-framework
Framework for supporting coding best practices.


## Version control history

**Mar 11, 2020 - 10.3.0 Minor change to the audit decorator**

- Minor change to event types defined in the Audit.NET object service audit decorator.
- Updated NuGet package version numbers that were missed in the last release, as well as references to them.

**Mar 5, 2020 - 10.2.0 Created read-only repository interface**

- Created a read-only repository interface and refactored the Azure Storage, Entity Framework and Entity Framework Core repository implementations accordingly.
- Added an integration project for Audit.NET and included an audit decorator class.
- Added an extension methhod for the Type class to determine whether a class implements or derives from another (including open and unbound generic types).
- Updated NuGet packages.

**Feb 14, 2020 - 10.1.0 Enhanced the Identity User Manager**

- Added a RestSharp tests project.
- Enhanced the Identity User Manager to allow verification of user tokens.
- Updated NuGet packages and removed redundant NuGet packages.

**Jan 8, 2020 - 10.0.0 Updated the targeted frameworks of all projects**

- Updated all projects to support a greater number of targeted frameworks. Updated any NuGet packages that needed updating.
- Added a method to the Identity User Manager for verifying two factor authentication tokens.
- Reverted the AspNet and EntityFramework projects back to only supporting .NET Framework.
- Removed redundant reference to the slf4net NuGet package in the AzureStorage project.
- Updated deprecated RestSharp methods.
- Updated copyright date on all projects.

**Dec 4, 2019 - 9.1.1 Updated .NET Standard projects to support .NET Framework 4.7.2**

- Updated all .NET Standard projects to also support .NET Framework 4.7.2.

**Dec 4, 2019 - 9.1.0 Updated .NET Standard and .NET Core versions for all projects**

- Updated the .NET Standard and .NET Core versions used by all projects.
- Created a ConfigurationManager helper class.

**Aug 8, 2019 - 9.0.0 Converted the AspNet and EntityFramework projects to .NET Standard**

- Converted the Tardigrade.Framework.AspNet and Tardigrade.Framework.EntityFramework projects from .NET Framework to .NET Standard to resolve ongoing issues with NuGet Pack.

**Aug 7, 2019 - 8.2.1 Resolved an issue with the NuGet pack of version 8.2.0**

- Repacked (NuGet) the Tardigrade.Framework.AspNet project using the Debug configuration rather than the Release configuration as it is a .NET Framework project.

**Aug 6, 2019 - 8.2.0 Created a custom role manager for ASP.NET Identity**

- Created a custom role manager for ASP.NET and ASP.NET Core Identity.

**Aug 1, 2019 - 8.1.1 Resolved an issue with the NuGet pack of version 8.1.0**

- Resolved an issue with the NuGet pack of version 8.1.0 whereby the DLL was being referenced from the Debug rather than Release directory (https://github.com/NuGet/Home/issues/7079).

**July 31, 2019 - 8.1.0 Resolved an argument null exception with the repository layer**

- Resolved an issue with the repository layer when retrieving an object by identifier for an object that does not exist.

**July 29, 2019 - 8.0.1 Updated referenced NuGet packages for all projects**

- Updated referenced NuGet packages for all projects.

**July 26, 2019 - 8.0.0 Redesigned the repository layer to better manage bulk operations**

- Enhanced the repository layer to support bulk operations on delete and update.
- Removed operations that delete objects by unique identifier from the repository and service layers.
- Resolved issue regarding the use of "includes" with the DbSet<T>.Find() method.
- Added a helper class for executing asynchronous methods synchronously.

**June 27, 2019 - 7.0.0 Redesigned the Identity services to make them easier to use**

- Standardised the Identity user framework to make it more intuitive and easier to implement based upon Microsoft conventions.
- Added paging and sorting parameters to the GET action of ObjectController.
- Modified SimpleInjectorServiceContainer to enable basic chaining.

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
