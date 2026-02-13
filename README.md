# OgcApi.Net

.NET implementation of the OGC API family of standards

[![GitHub](https://img.shields.io/github/license/sam-is/OgcApi.Net)](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE)

## Overview

OgcApi.Net is a .NET implementation of the OGC API family of standards for working with geospatial data. It supports modular API building blocks to spatially enable Web APIs in a consistent way, using the OpenAPI specification as the foundation.

Key features:
- Implements OGC API - Features, Tiles, and Schemas standards.
- Supports multiple data providers: SQL Server, PostgreSQL/PostGIS, SQLite/SpatiaLite, and MbTiles.
- Provides automatic API generation from metadata descriptions.
- Includes Swagger/OpenAPI documentation for API exploration and testing.

For detailed documentation, visit the [project documentation on GitHub Pages](https://sam-is.github.io/OgcApi.Net).

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
| OgcApi.Net.Styles | Styles standard implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.Styles)](https://www.nuget.org/packages/OgcApi.Net.Styles/) |

## Installation

To get started with OgcApi.Net, you need to install the core package and at least one data provider package depending on your database or data source. Below are the available NuGet packages:

### Core Package
The core package provides the basic implementation of the OGC API standards but does not include specific data providers. Install it using:

```bash
dotnet add package OgcApi.Net
```

### Data Provider Packages
Depending on your data source, install one or more of the following data provider packages:

- **SQL Server**: For working with Microsoft SQL Server or Azure SQL Database.
  ```bash
  dotnet add package OgcApi.Net.SqlServer
  ```

- **PostgreSQL/PostGIS**: For working with PostgreSQL databases with PostGIS extension.
  ```bash
  dotnet add package OgcApi.Net.PostGis
  ```

- **SQLite/SpatiaLite**: For working with SQLite databases with SpatiaLite extension.
  ```bash
  dotnet add package OgcApi.Net.SpatiaLite
  ```

- **MbTiles**: For working with vector tiles stored in the MbTiles format.
  ```bash
  dotnet add package OgcApi.Net.MbTiles
  ```

### Additional Dependencies for SpatiaLite
If you are using the SpatiaLite provider, you will need to add additional dependencies based on your operating system:

- **Windows**:
  ```bash
  dotnet add package Microsoft.Data.SQLite
  dotnet add package mod_spatialite
  ```

- **Linux/macOS**:
  ```bash
  dotnet add package SQLitePCLRaw.bundle_sqlite3
  ```

Additionally, if deploying to Linux or macOS, it is recommended to install the SpatiaLite library on your operating system:

- **Debian/Ubuntu**:
  ```bash
  apt-get install libsqlite3-mod-spatialite
  ```

- **macOS**:
  ```bash
  brew install libspatialite
  ```

For more details about installation and configuration, see the [Installation Guide](https://sam-is.github.io/OgcApi.Net/installation).

## Quick Start

1. Create an ASP.NET Core WebAPI project.
2. Install the required NuGet packages.
3. Configure the API in `Program.cs`:

   ```csharp
   builder.Services.AddOgcApi("ogcapi.json");
   ```

4. Add controllers:

   ```csharp
   builder.Services.AddControllers().AddOgcApiControllers();
   ```

5. Run the application and access the OpenApi specification at `/api/ogc/openapi.json`.

6. Optionally, configure Swagger UI or Scalar:

   ```csharp
   // Swagger UI (default endpoint /swagger)
   app.UseSwaggerUI(options => options.SwaggerEndpoint("/api/ogc/openapi.json", "OGC API"));

   // Scalar (default endpoint /scalar)
   app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("api/ogc/openapi.json"));
   ```

For a full setup guide, refer to the [API Configuration Documentation](https://sam-is.github.io/OgcApi.Net/configuration).

## Contributing

We welcome contributions from the community! Whether you're fixing bugs, improving documentation, or adding new features, your help is greatly appreciated.

Here are some ways you can contribute:
- **Bug Reports**: If you encounter any issues, please open a new issue in the [GitHub Issues](https://github.com/sam-is/OgcApi.Net/issues) section.
- **Feature Requests**: Have an idea for a new feature? Let us know by creating an issue or submitting a pull request.
- **Documentation Improvements**: Help us improve the documentation by suggesting edits or clarifications.
- **Code Contributions**: Fork the repository, make your changes, and submit a pull request. Please ensure your code follows the project's coding standards.

Before contributing, please take a moment to review the following:
- Ensure your changes align with the project's goals and standards.
- Write clear and concise commit messages.
- Include tests for any new functionality or bug fixes.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE) file for details.