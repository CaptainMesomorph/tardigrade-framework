# tardigrade-framework
Framework for supporting coding best practices.


##Version control history

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
