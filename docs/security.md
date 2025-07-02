# Authorization and Security

You can restrict access to features by providing predicates that will be included in the `WHERE` statement for all database queries. To do this, include the following settings in the features storage configuration:

- **ApiKeyPredicateForGet**: Predicate for `GET` requests.
- **ApiKeyPredicateForCreate**: Predicate for `CREATE` requests.
- **ApiKeyPredicateForUpdate**: Predicate for `UPDATE` requests.
- **ApiKeyPredicateForDelete**: Predicate for `DELETE` requests.

All predicates can contain the `@ApiKey` parameter, which is used to filter allowed features in the data source. This parameter can represent, for example, a user name or session ID.

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

For more details about the Features API, see [Features API](features-api.md).