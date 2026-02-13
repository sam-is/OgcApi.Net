---
layout: default
title: API Configuration
nav_order: 4
---

# API Configuration

This implementation supports automatic API generation from metadata descriptions. To generate the API, you must:

1. Create an ASP.NET Core WebAPI project.
2. Install the necessary NuGet packages.
3. Register providers in the `ConfigureServices` method. For example, to publish data from PostgreSQL, add the PostGis provider:
   ```csharp
   services.AddOgcApiPostGisProvider();
   ```
4. Register OpenAPI and configure the API:
   ```csharp
   services.AddOgcApi("ogcapi.json");
   ```
5. Add controllers in the `ConfigureServices` method:
   ```csharp
   services.AddControllers().AddOgcApiControllers();
   ```
6. Optionally, configure Swagger UI or Scalar:

   ```csharp
   // Swagger UI (default endpoint /swagger)
   app.UseSwaggerUI(options => options.SwaggerEndpoint("/api/ogc/openapi.json", "OGC API"));

   // Scalar (default endpoint /scalar)
   app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("api/ogc/openapi.json"));
   ```

This implementation uses attribute routing. All API endpoints will be accessible via the `/api/ogc` path.

API configuration can be done using a configuration file named `ogcapi.json` or `ogcsettings.json`, which has the following structure:

### Options example

```json
{
  "UseApiKeyAuthorization": true,
  "OpenApiVersion": "3.0",
  "LandingPage": {
    "Title": "OGC API Implementation",
    "Description": "The implementation of the OGC API family of standards that being developed to make it easy for anyone to provide geospatial data to the web",
    "Version": "1.0",
    "ContactName": "OGC API",
    "ContactUrl": "https://www.example.com/",
    "ApiDocumentPage": "/api/ogc/index.html",
    "ApiDescriptionPage": "/api/ogc/swagger.json"
  },
  "Conformance": {
    "ConformsTo": [
      "https://api.com/conform1.html",
      "https://api.com/conform2.html"
    ]
  },
  "Collections": {
    "Items": [
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
}
```

### Root-level configuration options

- **UseApiKeyAuthorization** (boolean, default: `false`)  
  Adds a security schema for API key authorization via query parameters to the OpenAPI specification.
- **OpenApiVersion** (string, default: `3.1`)  
  Specifies the OpenAPI specification version. Supported values: `2.0`, `3.0`, `3.1`, or `3.2`.

The Landing page element provides links to:
- the API definition (Swagger documentation and JSON description pages)
- the Conformance declaration (path /conformance, link relation conformance), and
- the Collections (path /collections, link relation data).

### Landing page options

**Required:**
- **Title**
- **Description**
- **Version** - API version
- **ContactName** - name of the data owner or API developer
- **ContactUrl** - URL to the data owner or API developer site
- **ApiDocumentPage** - URL to the API definition (Swagger or custom HTML page with API description)
- **ApiDescriptionPage** - URL to the API documentation (OpenAPI JSON)

**Optional:**
- **LicenseName** - Name of the license
- **LicenseUrl** - URL to the license definition
- **Links** - A list of other links

The Conformance declaration states the conformance classes from standards or community specifications identified by a URI that the API conforms to.

### Collection options

**Required:**
- **Id** - unique identifier of the collection
- **Title**
- Features options that depend on the data provider
- Tiles options that depend on the data provider

Collection can be:
- **Features only** - All data will be published as GeoJson objects
- **Tiles only** - Collection in this case will be published as MapBox Vector Tiles
- **Hybrid (features + tiles)** - API consumer can use tiles API for fast data queries and features API to get precise objects coordinates or modify objects

Tiles and features providers for one collection can be different. For example, you can create collection that publishes features from the database, but the tiles can be taken from mbtiles file.

**Optional:**
- **Description**
- **Links** - a list of other links
- **Extent** - the spatial and/or temporal extent of the collection
- **ItemType** - indicator of the type of items in the collection, defaults to `feature`
- **CalculateNumberMatched** - a boolean flag indicating if the number of matched items by a query should be returned in the response. It is recommended to set to `false` for large tables. Defaults to `true`
