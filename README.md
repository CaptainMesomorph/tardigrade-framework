# tardigrade-framework
Framework for supporting coding best practices.


##Version control history

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

**May 7, 2019 - 3.1.0 Resolve an EF/LINQ issue with the repository layer**

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
