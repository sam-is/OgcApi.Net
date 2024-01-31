# OgcApi.Net
.Net implementation of the OGC API family of standards

[![GitHub](https://img.shields.io/github/license/sam-is/OgcApi.Net)](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE)

## Overview

OGC API standards define modular API building blocks to spatially enable Web APIs in a consistent way. The OpenAPI specification is used to define the API building blocks.

Currently, this project implements the following standards:

Standard | Data Providers
--- | ---
[OGC API - Features - Part 1: Core](http://www.opengis.net/doc/IS/ogcapi-features-1/1.0) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis
[OGC API - Features - Part 2: Coordinate Reference Systems by Reference](http://www.opengis.net/doc/IS/ogcapi-features-2/1.0) | Independent 
[OGC API - Features - Part 4: Create, Replace, Update and Delete](http://docs.ogc.org/DRAFTS/20-002.html) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis 
[OGC API - Tiles - Part 1: Core](http://docs.ogc.org/DRAFTS/20-057.html) | Sqlite/MbTiles 

This project uses:
- ASP.NET Core 8 for building web API
- [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) for the features representation
- [ProjNET](https://github.com/NetTopologySuite/ProjNet4GeoAPI) for coordinate conversions
- [OpenAPI.NET](https://github.com/Microsoft/OpenAPI.NET) for OpenAPI documents generation

NuGet packages:

Package | Description | Link
--- | --- | ---
OgcApi.Net | OGC API - Features implementation without specific implementations of the data providers | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net)](https://www.nuget.org/packages/OgcApi.Net/)
OgcApi.Net.SqlServer | Sql Server features data provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.SqlServer)](https://www.nuget.org/packages/OgcApi.Net.SqlServer/)
OgcApi.Net.PostGis | PostgreSQL/PostGis features data provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.PostGis)](https://www.nuget.org/packages/OgcApi.Net.PostGis/)
OgcApi.Net.MbTiles | MbTiles tiles provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.MbTiles)](https://www.nuget.org/packages/OgcApi.Net.MbTiles/)

## API configuration

This implementation supports automatic API generation from metadata descriptions. To generate API, you must:
1. Create ASP.NET Core WebAPI project
2. Install necessary NuGet packages
3. Register providers in ConfigureServices method. For example, to publish data from PostgreSQL you must add PostGis provider:
```csharp
services.AddOgcApiPostGisProvider();
```
4. Register OpenAPI and configure API:
```csharp
services.AddOgcApi("ogcapi.json");
```
3. Add controllers in ConfigureServices method:
```csharp
services.AddControllers().AddOgcApiControllers();
```
This implementation uses attribute routing. All API endpoints will be accessible by the path /api/ogc.

API configuration can be made by configuration file named ogcsettings.json that has the following structure:

<details>
  <summary>Configuration example</summary>
  
```json
{  
  "LandingPage": {
    "Title": "OGC API Implementation",
    "Description": "The implementation of the OGC API family of standards that being developed to make it easy for anyone to provide geospatial data to the web",
    "Version": "1.0",
    "ContactName": "OGC API",
    "ContactUrl": "https://www.example.com/",
    "ApiDocumentPage": "/api/ogc/index.html",
    "ApiDescriptionPage": "/api/ogc/swagger.json"
  },
  "Collections": [
    {
      "Id": "Test",
      "Title": "Test collection",
      "Features": {
        "Crs": [
          "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
          "http://www.opengis.net/def/crs/EPSG/0/3857"
        ],
        "StorageCrs": "http://www.opengis.net/def/crs/EPSG/0/3857",
        "Storage": {
          "Type": "PostGis",
          "ConnectionString": "Host=localhost;User Id=postgre;Password=myStrongP@ssword;Database=Tests;Port=5432;Timeout=50;",
          "Schema": "test",
          "Table": "test_table",
          "GeometryColumn": "geom",
          "GeometrySrid": 3857,
          "GeometryDataType": "geometry",
          "GeometryGeoJsonType": "MultiPolygon",
          "IdentifierColumn": "id",
          "Properties": [
            "name",
            "region"
          ]
        }
      },
      "Tiles": {
        "Crs": "http://www.opengis.net/def/crs/EPSG/0/3857",
        "Storage": {
          "Type": "MbTiles",
          "FileName": "TilesData\\data.mbtiles"
        }
      }
    }
  ]
}
```
</details>

The Landing page element provides links to:
- the API definition (Swagger documentation and JSON description pages)
- the Conformance declaration (path /conformance, link relation conformance), and
- the Collections (path /collections, link relation data).

The Conformance declaration states the conformance classes from standards or community specifications identified by a URI that the API conforms to.

In the landing page options, you must specify:
- **Title**
- **Description**
- **Version** - API version
- **ContactName** - name of the data owner or API developer
- **ContactUrl** - URL to the data owner or API developer site
- **ApiDocumentPage** - URL to the API definition (Swagger or custom HTML page with API description)
- **ApiDescriptionPage** - URL to the API documentation (OpenAPI JSON)

In collection options, you must specify:
- **Id** - unique identifier of the collection
- **Title**
- Features options that dependents on the data provider
- Tiles options that dependents on the data provider

Collection can be:
- features only. All data will be published as GeoJson objects
- tiles only. Collection in this case will be published as MapBox Vector Tiles
- hybrid: features + tiles. That means that API consumer can use tiles API for fast data queries and features API to get precise objects coordinates or modify objects

Tiles and features providers for one collection can be different. For example, you can create collection that publishes features from the database, but the tiles can be taken from mbtiles file.

## Data providers configuration

### Features API configuration

OGC API - Features is a multi-part standard that offers the capability to create, modify, and query spatial data on the Web. It specifies requirements and recommendations for APIs that want to follow a standard way of sharing feature data.

OGC API Features provide access to collections of geospatial data based on OpenAPI 3.

Currently, only the database providers are supported: Sql Server, PostgreSQL/PostGis.

Use can publish geospatial data to the web from any table or view. Each table or view is treated as a separate data source.
To provide settings, you must set options.

Feature collection options include: 
- **Crs** - supported coordinate systems for the operations
- **StorageCrs** - coordinate system used by data provider to store features

Storage options include:
- **ConnectionString** - connection string for the source database. Defined for each source or table
- **Schema** - table or view schema
- **Table** - table name
- **GeometryColumn** - name of the column in the table containing spatial data
- **GeometryDataType** - can be geometry or geography
- **GeometryGeoJsonType** - OGC compatible geometry type, used to make OpenAPI GeoJson definition
- **IdentifierColumn** - name of the identifier column in the table
- **Properties** - array of any additional columns to publish

<details>
  <summary>Options example</summary>
  
```json
{
  "Id": "Test",
  "Title": "Test collection",
  "Features": {
	"Crs": [
	  "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
	  "http://www.opengis.net/def/crs/EPSG/0/3857"
	],
	"StorageCrs": "http://www.opengis.net/def/crs/EPSG/0/3857",
	"Storage": {
	  "Type": "PostGis",
	  "ConnectionString": "Host=localhost;User Id=postgre;Password=myStrongP@ssword;Database=Tests;Port=5432;Timeout=50;",
	  "Schema": "test",
	  "Table": "test_table",
	  "GeometryColumn": "geom",
	  "GeometrySrid": 3857,
	  "GeometryDataType": "geometry",
	  "GeometryGeoJsonType": "MultiPolygon",
	  "IdentifierColumn": "id",
	  "Properties": [
		"name",
		"region"
	  ]
	}
  }
}
```
</details>

### Create, Replace, Update and Delete operations

By default, all collections defined in the configuration file support only GET requests.
To allow data modification operation to the features, you must include in the data provider configuration the following elements:
- **AllowCreate** - to allow Create/Insert operation for the collection
- **AllowReplace** - to allow Replace operation for the collection
- **AllowUpdate** - to allow Update operation for the collection
- **AllowDelete** - to allow Delete operation for the collection

Defining one or more of these options automatically expands API to the OGC API - Features - Part 4: Create, Replace, Update and Delete

<details>
  <summary>Options example</summary>
  
```json
{
  "Id": "Test",
  "Title": "Test collection",
  "Features": {
	"Crs": [
	  "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
	  "http://www.opengis.net/def/crs/EPSG/0/3857"
	],
	"StorageCrs": "http://www.opengis.net/def/crs/EPSG/0/3857",
	"Storage": {
	  "Type": "PostGis",
	  "ConnectionString": "Host=localhost;User Id=postgre;Password=myStrongP@ssword;Database=Tests;Port=5432;Timeout=50;",
	  "Schema": "test",
	  "Table": "test_table",
	  "GeometryColumn": "geom",
	  "GeometrySrid": 3857,
	  "GeometryDataType": "geometry",
	  "GeometryGeoJsonType": "MultiPolygon",
	  "IdentifierColumn": "id",
	  "Properties": [
		"name",
		"region"
	  ]
	  "AllowCreate": true,
	  "AllowUpdate": true
	}
  }
}
```
</details>

### API Authorization

You can restrict access to the features by providing predicates that will be included in WHERE statement for all database queries. To do this you must include in the features storage configuration the following settings: **ApiKeyPredicateForGet**, **ApiKeyPredicateForCreate**, **ApiKeyPredicateForUpdate**, **ApiKeyPredicateForDelete**.

All predicates can contain @ApiKey parameter that is used to filter allowed features in the data source. This parameter can be, for example, user name or session id. 

<details>
  <summary>Options example</summary>
  
```json
{
  "Id": "Test",
  "Title": "Test collection",
  "Features": {
	"Crs": [
	  "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
	  "http://www.opengis.net/def/crs/EPSG/0/3857"
	],
	"StorageCrs": "http://www.opengis.net/def/crs/EPSG/0/3857",
	"Storage": {
	  "Type": "PostGis",
	  "ConnectionString": "Host=localhost;User Id=postgre;Password=myStrongP@ssword;Database=Tests;Port=5432;Timeout=50;",
	  "Schema": "test",
	  "Table": "test_table",
	  "GeometryColumn": "geom",
	  "GeometrySrid": 3857,
	  "GeometryDataType": "geometry",
	  "GeometryGeoJsonType": "MultiPolygon",
	  "IdentifierColumn": "id",
	  "Properties": [
		"name",
		"region"
	  ]
	  "AllowCreate": true,
	  "AllowUpdate": true,
	  "ApiKeyPredicateForGet": "EXISTS(SELECT * FROM users WHERE id = @ApiKey",
	  "ApiKeyPredicateForCreate": "EXISTS(SELECT * FROM users WHERE id = @ApiKey",
	  "ApiKeyPredicateForUpdate": "EXISTS(SELECT * FROM users WHERE id = @ApiKey"
	}
  }
}
```
</details>

### Tiles API

Current implementation supports only MapBox Vector Tiles to publish through API. Vector tiles must be stored in MbTiles format. You can generate tiles from GeoJson files by  [tippecanoe](https://github.com/mapbox/tippecanoe).

You can add tiles API to the existing collection or create a new collection that will contain only tiles without features API. 

Tiles API options include:
- Crs - coordinate system used to store tiles
- Type - currently only MbTiles supported
- FileName - path to the MbTiles file

<details>
  <summary>Options example</summary>
  
```json
{
  "Id": "Test",
  "Title": "Test collection",
  "Features": {
	"Crs": [
	  "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
	  "http://www.opengis.net/def/crs/EPSG/0/3857"
	],
	"StorageCrs": "http://www.opengis.net/def/crs/EPSG/0/3857",
	"Storage": {
	  "Type": "PostGis",
	  "ConnectionString": "Host=localhost;User Id=postgre;Password=myStrongP@ssword;Database=Tests;Port=5432;Timeout=50;",
	  "Schema": "test",
	  "Table": "test_table",
	  "GeometryColumn": "geom",
	  "GeometrySrid": 3857,
	  "GeometryDataType": "geometry",
	  "GeometryGeoJsonType": "MultiPolygon",
	  "IdentifierColumn": "id",
	  "Properties": [
		"name",
		"region"
	  ]
	}
  },
  "Tiles": {
  	"Crs": "http://www.opengis.net/def/crs/EPSG/0/3857",
	"Storage": {
	  "Type": "MbTiles",
	  "FileName": "TilesData\\data.mbtiles"
	}
  }
}
```
</details>

### Swagger generation

[Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) can be used for the Swagger web page automatic generation
To do this add Swagger configuration in Configure method of your Startup class:

```csharp
app.UseSwaggerUI(swaggerOptions =>
{
    swaggerOptions.RoutePrefix = "api";
    swaggerOptions.SwaggerEndpoint("ogc/swagger.json", "OGC API");
});
```

OpenAPI json definition is available on /api/ogc/swagger.json route in your application. 

### CORS support

All OGC API controllers use policy with "OgcApi" name. Policy can be configured in ConfigureServices method:

```csharp
services.AddCors(c => c.AddPolicy(name: "OgcApi", options =>
{
    options.AllowAnyMethod().AllowAnyHeader();
}));
```

Don't forget to add 
```csharp
app.UseCors("OgcApi");
```
in Configure method.

### Coordinate systems

API supports any coordinate system identified by SRID. Each coordinate system must have corresponding URI.
To add your custom coordinate system, modify file SRID.csv provided by the NuGet package. 

### Tests

Currently, this project contains tests for the data providers. Testing the entire API can be done by [OGC API - Features Conformance Test Suite](https://cite.opengeospatial.org/te2/about/ogcapi-features-1.0/1.0/site/)
