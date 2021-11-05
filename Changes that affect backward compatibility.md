# tardigrade-framework
Framework for supporting coding best practices.


## Breaking changes

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
