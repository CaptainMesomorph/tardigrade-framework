# tardigrade-framework
Framework for supporting coding best practices.


## Multiple target framework support

The following article outlines some of the reasoning for the support of .NET Framework 4.7.2, .NET Standard 2.0, .NET 5 and .NET 6. Verions before .NET Framework 4.7.2 are no longer explicitly supported.

> [The future of .NET Standard](https://devblogs.microsoft.com/dotnet/the-future-of-net-standard/)

## Breaking changes

**16.0.0 -> 17.0.0**

- Tardigrade.Framework
  - Updated ApplicationConfiguraton.cs to make support of User Secrets more intuitive. This included replacing the EntryAssembly virtual property with the UserSecretsAssembly protected variable.
  - Updated the UnitTestClassFixture.cs class to implement IServiceContainer and made the Services property protected.
- Tardigrade.Framework.AuditNET
  - Moved the AzureQueueDataProvider.cs class from TikForce.Framework.AuditNET project to the newly added TikForce.Framework.AuditNET.AzureStorageQueue project. Refactored references accordingly.
  - Upgraded the AzureQueueDataProvider.cs class to use the Azure.Storage.Queues package instead of the deprecated Microsoft.Azure.Storage.Queue package. Updated the code accordingly.
- Tardigrade.Framework.EntityFramework.Core
  - Made the DbHelper.GenerateCreateScript() method into an extension method of the DbContextExtension.cs class.

**15.0.0 -> 16.0.0**

- Tardigrade.Framework
  - Added the "class" contraint to the TContent generic parameter for Post and Put methods of the IRestClient interface.
- Dropped .NET 5 and .NET Framework 4.7.2 as targeted frameworks for the following projects:
  - Framework
  - Framework.AuditNET
  - Framework.AzureStorage
  - Framework.MailKit
  - Framework.RestSharp
  - Framework.SimpleInjector
  - Framework.ZXingNet

**14.1.0 -> 15.0.0**

- Tardigrade.Framework
  - The DisplayName property of the EmailAddress class has been renamed to Name.
- Tardigrade.Framework.EntityFrameworkCore
  - Deleted the IdentityUserDbContext.cs class. As a result, the Tardigrade.Framework.EntityFrameworkCore project now references the Tardigrade.Framework project instead of the Tardigrade.Framework.AspNetCore project.
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

**13.1.0 -> 14.0.0**

- Tardigrade.Framework
  - The IRestClient interface has changed significantly.
- Tardigrade.Framework.RestSharp
  - The RestSharpClient class has changed significantly due to changes to the IRestClient interface.
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

**12.1.0 -> 13.0.0**

- Tardigrade.Framework
  - Renamed the LegacyConfigurationProvider class to LegacySettingsConfigurationProvider.
  - Renamed the LegacyConfigurationSource class to LegacySettingsConfigurationSource.
  - Updated the signature of the ApplicationConfiguration constructor to accept multiple configuration sources.
- Tardigrade.Framework.AspNetCore
  - Updated the project from supporting both .NET Core 2.2 and 3.0 to just 3.1.
- Tardigrade.Framework.EntityFrameworkCore
  - Updated the project from .NET Core 3.0 to 3.1.
