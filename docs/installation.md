# Installation

## Required Dependencies
To use SpatiaLite, you will need to add additional NuGet package dependencies to your project:
- For Windows: `Microsoft.Data.SQLite` and `mod_spatialite`.
- For Linux and macOS: `SQLitePCLRaw.bundle_sqlite3`.

If you are deploying your application on Linux or macOS, it is recommended to install the SpatiaLite library on your operating system:
- Debian/Ubuntu:
  ```bash
  apt-get install libsqlite3-mod-spatialite
  ```
- macOS:
  ```bash
  brew install libspatialite
  ```

## NuGet Packages
The following packages are available:

| Package | Description | Link |
| --- | --- | --- |
| OgcApi.Net | OGC API - Features implementation without specific data providers | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net)](https://www.nuget.org/packages/OgcApi.Net/) |
| OgcApi.Net.SqlServer | SQL Server features data provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.SqlServer)](https://www.nuget.org/packages/OgcApi.Net.SqlServer/) |
| OgcApi.Net.PostGis | PostgreSQL/PostGis features data provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.PostGis)](https://www.nuget.org/packages/OgcApi.Net.PostGis/) |
| OgcApi.Net.SpatiaLite | SQLite/SpatiaLite features data provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.SpatiaLite)](https://www.nuget.org/packages/OgcApi.Net.SpatiaLite/) |
| OgcApi.Net.MbTiles | MbTiles tiles provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.MbTiles)](https://www.nuget.org/packages/OgcApi.Net.MbTiles/) |
| OgcApi.Net.Schemas | Schemas standard implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.Schemas)](https://www.nuget.org/packages/OgcApi.Net.Schemas/) |

For API configuration, see [API Configuration](configuration.md).