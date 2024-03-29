# tardigrade-framework
Framework for supporting coding best practices.


## Version control history

**Sept 22, 2023 - 19.0.0 Updated NuGet packages for all projects**

- Updated third-party NuGet packages for all projects.
- Resolved backward compatibility issues encountered with the packages update.
- Made enhancements to the ConfigurationExtension and EnumExtension classes.
- Implemented additional extension classes.
- Added unit tests to support extension class additions and enhancements.
- Enhanced the UnitTestClassFixture class to resolve issues with the seeding of test data.
- Added a Boolean to Integer converter XML text writer.
- Updated the Tardigrade.Framework project to support the latest language version.
- Updated the Tardigrade.Framework.EntityFrameworkCore project to also support .NET 7.0.
- In Tardigrade.Framework, changed the exception thrown from the ConfigurationExtension.GetAsEnum<TEnum>() method from ArgumentException to FormatException.
- In Tardigrade.Framework, enhanced the UnitTestClassFixture class to enable the use of services straight after their configuration.
- In Tardigrade.Framework.AspNetCore, resolved issues with the deprecated Microsoft.AspNetCore.Identity and Microsoft.AspNetCore.Mvc.Core packages - https://github.com/dotnet/aspnetcore/issues/49101.

**Apr 21, 2022 - 18.0.0 Updated target framework of Tardigrade.Framework.EntityFramework**

- Added .NET Standard 2.1 as a target framework to the Tardigrade.Framework.EntityFramework project.
- Created string extension methods to convert to and from Base64. Updated the AzureQueueDataProvider to ensure messages written to the a queue are Base64.
- Updated third-party NuGet packages.

**Mar 9, 2022 - 17.0.0 Auditing, application configuration and unit testing enhancements**

- Moved the AzureQueueDataProvider.cs class from TikForce.Framework.AuditNET project to the newly added TikForce.Framework.AuditNET.AzureStorageQueue project. Refactored references accordingly.
- Upgraded the AzureQueueDataProvider.cs class to use the Azure.Storage.Queues package instead of the deprecated Microsoft.Azure.Storage.Queue package. Updated the code accordingly.
- Updated ObjectServiceAuditDecorator.cs to better support inheritance.
- Updated ApplicationConfiguraton.cs to make support of User Secrets more intuitive.
- Added an Azure Storage specific version of the ApplicationConfiguraton.cs class.
- Updated the UnitTestClassFixture.cs class to implement IServiceContainer and made the Services property protected.
- Made the DbHelper.GenerateCreateScript() method into an extension method of the DbContextExtension.cs class.
- Properly implemented the IDisposable pattern in the UnitTestClassFixture.cs class.
- Added a new test project for Tardigrade.Framework.AuditNET and added unit tests for the audit decorator.
- Updated the Tardigrade.Framework.AzureStorage.Tests project to support User Secrets.
- Set the <Nulable> property for all .NET 6 SDK-style test projects to "enable" and refactored code where appropriate.
- Created the EntityFrameworkCoreClassFixture.cs class that the extends UnitTestClassFixture.cs class from the Tardigrade.Framework project. Refactored the BlogTest.cs and UserTest.cs classes to use this new class fixture.
- Deleted the deprecated DateTimeJsonConverter.cs and NewtonsoftJsonExtension.cs classes from the Tardigrade.Shared.Tests project.
- Updated third-party NuGet packages.

**Feb 17, 2022 - 16.0.0 Audit decorator enhancements**

- Made every method in the ObjectServiceAuditDecorator class virtual so that the class can be inherited and the methods overwritten. Enahnced the constructor to accept an ILogger instance and implemented appropriate logging.
- Added the "class" contraint to the TContent generic parameter for Post and Put methods of the IRestClient interface.
- Added an extension class for the ILogger interface.
- Updated the RestSharpClient and NewtonsoftJsonDeserializer classes due to backward incompatibility issues with the upgrade of the RestSharp NuGet package.
- Updated third-party NuGet packages.
- Dropped .NET 5 and .NET Framework 4.7.2 as targeted frameworks for the following projects:
  - Framework
  - Framework.AuditNET
  - Framework.AzureStorage
  - Framework.MailKit
  - Framework.RestSharp
  - Framework.SimpleInjector
  - Framework.ZXingNet

**Dec 24, 2021 - 15.0.0 .NET 6 upgrade**

- Deleted the IdentityUserDbContext.cs class from the Tardigrade.Framework.EntityFrameworkCore project. As a result, this project now references the Tardigrade.Framework project instead of the Tardigrade.Framework.AspNetCore project.
- Recreated the Tardigrade.Framework.EntityFramework.Tests project as a .NET Standard SDK-style project.
- Renamed the DisplayName property of the EmailAddress class to Name.
- General code clean-up based on ReSharper recommendations.
- Updated the following projects to support .NET 6:
  - Framework
  - Framework.AuditNET
  - Framework.AzureStorage
  - Framework.MailKit
  - Framework.RestSharp
  - Framework.SimpleInjector
  - Framework.ZXingNet
- Updated the following projects to .NET 6:
  - All test projects
  - Framework.AspNetCore
  - Framework.EntityFrameworkCore
- Replaced .NET Standard 2.1 with .NET Standard 2.0 in the following projects:
  - Framework
  - Framework.AuditNET
  - Framework.AzureStorage
  - Framework.RestSharp
  - Framework.SimpleInjector
  - Framework.ZXingNet
- Updated the following projects to support .NET Standard 2.0 and .NET Framework 4.7.2:
  - Framework.MailKit

**Nov 11, 2021 - 14.1.0 Updated the EnumExtension class**

- Enhanced the ToEnum<T>(string) and ToEnum<T>(string, T) methods of the EnumExtension class to cater for the Display attribute.

**Nov 5, 2021 - 14.0.0 .NET 5 upgrade; soft deletion and tenanting support**

- Created functionality to support soft deletion of records using Entity Framework Core and added associated unit tests.
- Created functionality to support tenanting.
- Enhanced the IRestClient interface and implementations to support async versions of existing methods.
- Added a MailKit integration project (with associated test project).
- Re-implemented the ApplicationConfiguration class to better match the default host builder implementation and to properly support User Secrets.
- Re-designed class fixtures for more flexible use in unit tests.
- Incorporated the use of User Secrets in the Framework tests project.
- Updated third-party NuGet packages.
- Updated the following projects to the .NET 5 target framework:
  - All test projects
  - Framework
  - Framework.AspNetCore
  - Framework.AuditNET
  - Framework.AzureStorage
  - Framework.EntityFrameworkCore
  - Framework.RestSharp
  - Framework.SimpleInjector
  - Framework.ZXingNet
- Dropped the .NET Standard 2.0 target framework from the following projects:
  - Framework
  - Framework.AuditNET
  - Framework.AzureStorage
  - Framework.RestSharp
  - Framework.SimpleInjector
  - Framework.ZXingNet
- Dropped the .NET Framework 4.6.2 target framework from the following projects:
  - Framework
  - Framework.AspNet
  - Framework.AuditNET
  - Framework.AzureStorage
  - Framework.EntityFrameork
  - Framework.RestSharp
  - Framework.SimpleInjector
  - Framework.ZXingNet

**Dec 8, 2020 - 13.1.0 Enhanced ConfigurationExtension to support more type casting options**

- Enhanced ConfigurationExtension to support more type casting options.
- Added unit tests for reading application settings that were of a type other than string.

**Dec 1, 2020 - 13.0.0 Enhanced ApplicationManager to support database application settings**

- Enhanced the ApplicationConfigration class to enable the use of multiple configuration sources.
- Created configuration sources for reading application settings from a database (for both Entity Framework and Entity Framework Core).
- Resolved an issue with "Includes" of associated entities when calling the retrieve methods of the repository code in the EntityFrameworkCore project.
- Updated the CreateBulk and CreateBulkAsync methods of the EntityFrameworkCore Repository class to enable creation of associated child entities.
- Created utility classes for the Expression class.
- Updated the EntityFrameworkCore project from .NET Core 3.0 to 3.1.
- Updated the AspNetCore project from supporting both .NET Core 2.2 and 3.0 to just 3.1.
- Renamed the LegacyConfigurationProvider and LegacyConfigurationSource classes to LegacySettingsConfigurationProvider and LegacySettingsConfigurationSource respectively.
- Added a project to the Tests solution for holding shared resources.
- Incorporated the Bogus third-party NuGet package for generating test data through the DataFactory.
- Reactored the unit tests for reading application settings to enable better code reuse.
- Created an EntityFramework test project containing unit tests for reading application settings from an SQLite database.
- Created an EntityFrameworkCore test project containing unit tests for reading application settings and performing object CRUD operations from an SQLite database.
- Added unit tests for the new utility classes.
- Updated NuGet packages.

**Nov 13, 2020 - 12.1.0 Created classes for managing application configuration settings**

- Created classes for managing application configuration settings.
- Moved the IdentityUserDbContext class from the EntityFramework project to the AspNet project.
- Removed the unused test project Tardigrade.Framework.EntityFramework.Tests.
- Added unit tests for the new application configuration functionality.
- Updated NuGet packages.

**Oct 30, 2020 - 12.0.0 Resolved issue with UpdateAsync method of the Audit Decorator**

- Resolved an issue with the UpdateAsync method of the Audit Decorator.
- Redesigned the IRepository interface so that it no longer extends the IBulkRepository interface. Instead inheriting classes need to implement IBulkRepository explicitly.
- Renamed the Repository classes of the EntityFrameworkCore project to remove the "EntityFrameworkCore" prefix.
- Renamed the Repository classes of the AzureStorage project to remove the "AzureStorage" prefix.
- Removed a redundant generic parameter from the IDtoService interface.
- General code clean-up based on ReSharper recommendations.
- Updated unit tests.
- Updated NuGet packages.

**Sept 18, 2020 - 11.4.0 Enhanced the Repository layer to handle derived types from a class hierarchy**

- Enhanced the Repository layer of the EntityFramework project to handle derived types from a class hierarchy.
- Renamed the Repository classes of the EntityFramework project to remove the "EntityFramework" prefix.

**Sept 15, 2020 - 11.3.0 Updated IdentityUserManager implementations to include DeleteAsync**

- Updated IdentityUserManager implementations to include a DeleteAsync() method.
- Updated NuGet packages.

**Sept 7, 2020 - 11.2.0 Replaced usage of GetAwaiter().GetResult()**

- Replaced usage of "GetAwaiter().GetResult()" with "AsyncHelper.RunSync(() => asyncMethod())" in the AzureStorage project due to upgrade issues moving to the Microsoft.Azure.Cosmos.Table NuGet package.

**Sept 1, 2020 - 11.1.0 Reverted Repository class changes from previous release**

- Reverted changes to the Create operations of the Repository class in the EntityFrameworkCore project that rejected object IDs set to their "default" values.

**Sept 1, 2020 - 11.0.0 General enhancements and code clean-up**

- Created an ASP.NET extension for managing Cookies.
- Enhanced the IdentityRoleManager to retrieve all roles.
- Enhanced the IdentityUserManager to access user roles.
- General code clean-up based upon ReSharper recommendations.
- Included user identifier claims into JSON Web Tokens generated by the JsonWebTokenService.
- Created a user context model in the AspNetCore project based upon IHttpContextAccessor.
- Replaced the deprecated WindowsAzure.Storage package from the AuditNET and AzureStorage projects.
- Updated the Create operations of the Repository class in the EntityFrameworkCore project to reject object IDs which are set to their "default" values.
- Updated NuGet packages.

**May 8, 2020 - 10.5.0 Created a project for QR Code processing**

- Added a new project that implements a QR Code processor based upon ZXing.Net.
- Migrated existing xUnit test projects from the Tardigrade Framework Solution to a new Solution.
- Updated NuGet packages.

**Mar 20, 2020 - 10.4.0 Enhanced the auditing framework**

- Added an Audit.NET Data Provider for Azure Storage Queues.
- Enhanced the Audit.NET object service audit decorator to reference the currently logged in user and to ignore auditing errors.
- Replaced use of the default operator with the default literal (introduced in C# 7.1).
- Updated all custom exceptions to properly cater for serialisation.
- Updated NuGet packages.

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
