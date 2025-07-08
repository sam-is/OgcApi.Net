---
layout: default
title: Sample Application
nav_order: 12
---

# Sample Application

The `SampleWebApplication` is a demonstration project that showcases the capabilities of OgcApi.Net. Below is an explanation of how it works and how to set it up.

## Running the Application

The `SampleWebApplication` is launched using **Aspire**, which orchestrates the deployment of dependent services such as databases. The application is configured to run with an `https` profile. During startup, the following components are initialized:

- **SQL Server**: A SQL Server instance is launched with preloaded data.
- **PostgreSQL (PostGIS)**: A PostgreSQL instance with PostGIS extension is also launched with preloaded data.

All OGC API settings can be found in the `ogcapi.json` file, located alongside the `SampleWebApplication`. The `ogcapi.json` configuration file contains essential settings for the OGC API implementation. 
When the application starts, connection strings are dynamically injected into the `OgcApiOptions` configuration object during its initialization process. 
This dynamic injection is facilitated by the Aspire framework, which resolves and populates the connection string parameters based on the runtime environment and service dependencies.

For static configurations, you can explicitly define connection strings directly in the `ogcapi.json` file. In such cases, the simplified configuration method can be used:

```csharp
builder.Services.AddOgcApi("ogcapi.json");
```

Alternatively, for more advanced scenarios requiring custom logic, you can implement a custom `ConfigureOgcApiOptions` method to handle the configuration programmatically.

## Prerequisites

To run the Aspire-based project, ensure the following:

- **Docker** is installed and running on your machine.
- When switching launch profiles, make sure to clean up any existing database volumes to avoid conflicts.
- In case of persistent database issues, delete the associated volumes manually.

## Supported Providers and Features

The sample application demonstrates the integration of the following OgcApi.Net packages:

- **OgcApi.Net.SqlServer**: Implements a features data provider for SQL Server databases. This package enables querying geospatial data stored in SQL Server using the OGC API - Features standard.
- **OgcApi.Net.PostGis**: Implements a features data provider for PostgreSQL databases with PostGIS support. This package allows querying geospatial data stored in PostgreSQL/PostGIS using the OGC API - Features standard.
- **OgcApi.Net.MbTiles**: Implements a tiles provider for serving map tiles from `.mbtiles` files. This package supports tile-based access to pre-generated map data.
- **OgcApi.Net.Schemas**: Implements support for the [OGC API - Features - Part 5: Schemas](https://portal.ogc.org/files/108199) standard. This package provides a standardized schema definition for describing geospatial data collections.

## Tile Access Delegates

The application includes two access delegates to control access to tiles:

### TilesAccessDelegate

This delegate restricts access to tiles based on the provided `apiKey`. Its signature is:

```csharp
bool TilesAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey);
```

**Example Implementation:**

```csharp
public static bool TilesAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey) => (collectionId ?? "") switch
{
    "PolygonsWithApiKey" when (apiKey ?? "").Equals("qwerty") && tileMatrix is >= 0 and <= 7 =>
        tileMatrix switch
        {
            0 => tileRow == 0 && tileCol == 0,
            1 => tileRow == 0 && tileCol == 1,
            2 => tileRow == 1 && tileCol == 2,
            3 => tileRow == 2 && tileCol == 5,
            4 => tileRow == 5 && tileCol == 10,
            5 => tileRow == 10 && tileCol == 20,
            6 => tileRow == 20 && tileCol == 40,
            7 => tileRow == 40 && tileCol is 81 or 82,
            _ => false
        },
    _ => true,
};
```

### FeatureAccessDelegate

This delegate restricts access to specific features within a tile based on the provided `apiKey`. Its signature is:

```csharp
bool FeatureAccessDelegate(string collectionId, IFeature feature, string apiKey);
```

**Example Implementation:**

```csharp
public static bool FeatureAccessDelegate(string collectionId, IFeature feature, string apiKey) => (collectionId ?? "") switch
{
    "FeatureAccessData" => apiKey == "admin" ||
        apiKey == "value" && feature.Attributes.Exists("value") &&
        (feature.Attributes["value"] is long and > 1200 ||
        feature.Attributes["value"] is > 100.0) ||
        feature.Attributes.Exists("roleAccess") && feature.Attributes["roleAccess"].ToString() == apiKey,
    _ => true,
};
```