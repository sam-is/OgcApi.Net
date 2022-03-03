# OgcApi.Net
.Net implementation of the OGC API family of standards

[![GitHub](https://img.shields.io/github/license/sam-is/OgcApi.Net)](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE)

## Overview

OGC API standards define modular API building blocks to spatially enable Web APIs in a consistent way. The OpenAPI specification is used to define the API building blocks.

Currently, this project implements the following standards:

Standard | Data Providers
--- | ---
[OGC API - Features - Part 1: Core](http://www.opengis.net/doc/IS/ogcapi-features-1/1.0) | Microsoft SQL Server 2012+ <br> Azure SQL Database
[OGC API - Features - Part 2: Coordinate Reference Systems by Reference](http://www.opengis.net/doc/IS/ogcapi-features-2/1.0) | Independent 

This project uses:
- ASP.NET Core 5.0 for building web API
- [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) for the features representation
- [ProjNET](https://github.com/NetTopologySuite/ProjNet4GeoAPI) for coordinate conversions
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) for OpenAPI and Swagger documents generation

NuGet packages:
Package | Description | Link
--- | --- | ---
OgcApi.Net | OGC API - Features implementation without specific implementations of the data providers | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net)](https://www.nuget.org/packages/OgcApi.Net/)
OgcApi.Net.SqlServer | Sql Server data provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.SqlServer)](https://www.nuget.org/packages/OgcApi.Net.SqlServer/)
OgcApi.Net.PostGis | PostgreSQL/PostGis data provider implementation | [![Nuget](https://img.shields.io/nuget/v/OgcApi.Net.PostGis)](https://www.nuget.org/packages/OgcApi.Net.Features.PostGis/)

## Features API

OGC API - Features is a multi-part standard that offers the capability to create, modify, and query spatial data on the Web. It specifies requirements and recommendations for APIs that want to follow a standard way of sharing feature data.
OGC API Features provides access to collections of geospatial data based on OpenAPI 3. 

This implementation supports automatic API generation from metadata descriptions. To generate API, you must:
1. Create ASP.NET Core WebAPI project
2. Install necessary NuGet packages
3. Add controllers in ConfigureServices method:
```csharp
services.AddControllers().AddOgñFeaturesControllers();
```
This implementation uses attribute routing. All API endpoints will be accessible by the path /api/ogc.

### API configuration

In code:
```csharp
services.AddOgcApi(options =>
{ 
    options.LandingPage = new LandingPageOptions()
    {
        Title = "OGC API Implementation",
        Description = "The implementation of the OGC API family of standards that being developed to make it easy for anyone to provide geospatial data to the web",
        ApiDocumentPage = new Uri("/swagger/index.html", UriKind.Relative),
        ApiDescriptionPage = new Uri("/swagger/ogc/swagger.json", UriKind.Relative)
    };
    options.Conformance = new ConformanceOptions()
    {
        ConformsTo = new List<Uri>()
        {
            new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core"),
            new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30"),
            new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/html"),
            new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/geojson")
        }
    };
    options.Collections = new CollectionsOptions()
    {
        Collections = new List<CollectionOptions>()
        {
            new CollectionOptions()
            {
                Id = "Test",
                Description = "This is test collection",
                Title = "Test collection",
                Extent = new Extent()
                {
                    Spatial = new SpatialExtent()
                    {
                        Bbox = new[] { new[] { 0, 0, 100000, 100000 } }
                    }
                },   
                SourceType = "SqlServer"
            }
        }
    };
});
```

Or in appsettings:

```json
"OgcApiOptions": {
  "LandingPage": {
    "Title": "OGC API Implementation",
    "Description": "The implementation of the OGC API family of standards that being developed to make it easy for anyone to provide geospatial data to the web",
    "ApiDocumentPage": "/swagger/index.html",
    "ApiDescriptionPage": "/swagger/ogc/swagger.json"
  },
  "Collections": {
    "Items": [
      {
        "SourceType": "SqlServer",
        "Id": "Test",
        "Title": "Test collection",
        "Description": "This is test collection",
        "Crs": [
          "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
          "http://www.opengis.net/def/crs/EPSG/0/28409",
          "http://www.opengis.net/def/crs/EPSG/0/3857"
        ],
        "StorageCrs": "http://www.opengis.net/def/crs/EPSG/0/28409"
      }
    ]
  }
}
```
and in code:
```csharp
services.AddOgcApi((settings) =>
{
    Configuration.GetSection("OgcApiOptions").Bind(settings);
});
```

The Landing page provides links to:
- the API definition (Swagger documentation and JSON description pages)
- the Conformance declaration (path /conformance, link relation conformance), and
- the Collections (path /collections, link relation data).

The Conformance declaration states the conformance classes from standards or community specifications identified by a URI that the API conforms to.

In the landing page options, you must specify:
- Title
- Description
- ApiDocumentPage - URL to the API definition (OpenAPI)
- ApiDescriptionPage - URL to the API documentation (Swagger JSON)

In collection options, you must specify:
- Id - unique identifier of the collection
- SourceType - currently only SqlServer supported
- Title
- Description
- Crs - coordinate systems supported by the collection
- StorageCrs - source coordinate system of the collection

### Data providers configuration 

### SQL Server provider

Use can publish geospatial data to the web from any table or view. Each table or view is treated as a separate data source.
To provide settings you must set options.
In code:
```csharp
services.AddOgcApiSqlServerProvider(options =>
{
    options.Sources = new List<SqlServerCollectionSource>()
    {
        new SqlServerCollectionSource()
        {
            Id = "Test",
            ConnectionString = "data source=localhost,1433;user id=sa;password=myStrongP@ssword;initial catalog=Tests;Persist Security Info=true",
            Schema = "dbo",
            Table = "TestTable",
            GeometryColumn = "geom",
            GeometryDataType = "geometry",
            IdentifierColumn = "ID",
            Properties = new List<string>() { "Name", "Region" }
        }
    };                
});
```

Or in appsettings:
```json
"OgcApiSqlServerOptions": {
  "Sources": [
    {
      "Id": "Test",
      "ConnectionString": "data source=localhost,1433;user id=sa;password=myStrongP@ssword;initial catalog=Tests;Persist Security Info=true",
      "Schema": "dbo",
      "Table": "TestTable",
      "GeometryColumn": "geom",
      "GeometryDataType": "geometry",
      "IdentifierColumn": "ID",
      "Properties": [
        "Name",
        "Region"
      ]
    }
  ]
}
```

Id property of the source options must match corresponding collection id.

SQL Server options:
- ConnectionString - connection string for the source database. Defined for each source or table
- Schema - table or view schema
- Table - table name
- GeometryColumn - name of the column in the table containing spatial data
- GeometryDataType - can be geometry or geography
- IdentifierColumn - name of the identifier column in the table
- Properties - any additional columns to publish

### Swagger generation

You can automatically generate OpenApiDocument from the routes, controllers and models.
To do this add:

```csharp
services.AddOgcApiSwaggerGen(); 
```

Document generation is based on C# xml code documentation in the file OgcApi.Net.xml provided with the NuGet package. 
Alternative is to supply your own swagger.json for the Swashbuckle.

### Coordinate systems

API supports any coordinate system identified by SRID. Each coordinate system must have corresponding URI.
To add your custom coordinate system modify file SRID.csv provided by the NuGet package. 

### Tests

Currently, this project contains tests for the data providers. Testing the entire API can be done by [OGC API - Features Conformance Test Suite](https://cite.opengeospatial.org/te2/about/ogcapi-features-1.0/1.0/site/)
