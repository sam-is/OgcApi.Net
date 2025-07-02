# Features API

OGC API - Features is a multi-part standard that provides the capability to create, modify, and query spatial data on the web. It specifies requirements and recommendations for APIs that want to follow a standard way of sharing feature data.

OGC API - Features provides access to collections of geospatial data based on OpenAPI 3. Currently, only database providers are supported: SQL Server, PostgreSQL/PostGis, and SQLite with the SpatiaLite extension.

You can publish geospatial data to the web from any table or view. Each table or view is treated as a separate data source.

## Feature Collection Options
Feature collection options include:
- **Crs**: Supported coordinate systems for operations.
- **StorageCrs**: Coordinate system used by the data provider to store features.

## Storage Options
Storage options include:
- **ConnectionString**: Connection string for the source database.
- **Schema**: Table or view schema.
- **Table**: Table name.
- **GeometryColumn**: Name of the column containing spatial data.
- **GeometryDataType**: Can be `geometry` or `geography`.
- **GeometryGeoJsonType**: OGC-compatible geometry type, used to define OpenAPI GeoJSON.
- **IdentifierColumn**: Name of the identifier column in the table.
- **Properties**: Array of additional columns to publish.

<details>
  <summary>Options Example</summary>
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

## Create, Replace, Update and Delete operations

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