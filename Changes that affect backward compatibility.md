# tardigrade-framework
Framework for supporting coding best practices.


## Breaking changes

**12.1.0 -> 13.0.0**

- Tardigrade.Framework
  - Renamed the LegacyConfigurationProvider class to LegacySettingsConfigurationProvider.
  - Renamed the LegacyConfigurationSource class to LegacySettingsConfigurationSource.
  - Updated the signature of the ApplicationConfiguration constructor to accept multiple configuration sources.
- Tardigrade.Framework.AspNetCore
  - Updated the project from supporting both .NET Core 2.2 and 3.0 to just 3.1.
- Tardigrade.Framework.EntityFrameworkCore
  - Updated the project from .NET Core 3.0 to 3.1.
