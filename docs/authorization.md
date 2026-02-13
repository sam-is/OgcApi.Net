---
layout: default
title: Authorization and Security
nav_order: 8
---

# Authorization and Security

## OpenAPI Specification

In the `ogcapi.json` options file, add:

```json
"UseApiKeyAuthorization": true
```

This adds a security schema for API key authorization via query parameters (`apiKey`) to the OpenAPI specification.

## SQL Database Authorization
You can restrict access to features by providing predicates that will be included in the `WHERE` statement for all database queries. To do this, include the following settings in the features storage configuration:

- **ApiKeyPredicateForGet**: Predicate for `GET` requests.
- **ApiKeyPredicateForCreate**: Predicate for `CREATE` requests.
- **ApiKeyPredicateForUpdate**: Predicate for `UPDATE` requests.
- **ApiKeyPredicateForDelete**: Predicate for `DELETE` requests.

All predicates can contain the `@ApiKey` parameter, which is used to filter allowed features in the data source. This parameter can represent, for example, a user name or session ID.

### Options example

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

For more details about the Features API, see [Features API](features-api.md).

## MBTiles Authorization

### Tile Access

To control tile access, implement the delegate:

```csharp
public delegate bool TileAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey);
```

Then configure it in `OgcApiOptions` for the desired collection:

```
collectionOptions..Tiles.Storage.TileAccessDelegate = TilesAccessDelegate
```

For an example of `TileAccessDelegate`, see [Sample Application](sample-application.md) 

### Feature Access
To control feature access within tiles, implement the delegate:

```csharp
public delegate bool FeatureAccessDelegate(string collectionId, IFeature feature, string apiKey);
```

Then configure it in `OgcApiOptions` for the desired collection:

```
collectionOptions..Tiles.Storage.FeatureAccessDelegate = FeatureAccessDelegate
```

For an example of `FeatureAccessDelegate`, see [Sample Application](sample-application.md) 