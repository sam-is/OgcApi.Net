# Schemas Support

Schema support is implemented in a separate library: `OgcApi.Net.Schemas`. It supports both SQL and MbTiles data providers.

Related documentation in the standard: [OGC API - Features - Part 5: Schemas](https://portal.ogc.org/files/108199).

## Enabling Schema Support
To enable schema support in your application:
1. Install the `OgcApi.Net.Schemas` module.
2. Register it using the `AddSchemasOpenApiExtension()` method **before** calling `AddOgcApi()`:
   ```csharp
   services.AddSchemasOpenApiExtension();
   services.AddOgcApi("ogcapi.json");
   ```

Once registered, the following endpoints will be available:

| Endpoint | Description |
| --- | --- |
| GET /collections/{collectionId}/schema | Returns the full JSON Schema describing features in the collection. |
| GET /collections/{collectionId}/queryables | Returns the schema with all queryable properties (response equals `/schema` path). |
| GET /collections/{collectionId}/sortables | Returns the schema with all sortable properties (currently always empty). |

## Schema Options
The `SchemaOptions` configuration includes:
- **Title**: Title of the collection.
- **Description**: Description of the collection.
- **AdditionalProperties**: Boolean flag indicating whether additional properties are allowed in feature objects.
- **Properties**: Dictionary mapping property names to their `PropertyDescription`.

### Property Description
Each property description includes:
- **Type**: Type of the property. The standard recommends using one of the following: `string`, `number`, `integer`, `boolean`, `object` or `array`.
- **Title**: Human-readable title for the property.
- **Description**: Description of the property.
- **XOgcRole**: Custom OGC role (`id`, `primary-geometry`, `type`, etc.).
- **Format**: Format of the property. For geometry properties, use one of: `geometry-point`, `geometry-multipoint`, `geometry-linestring`, `geometry-multilinestring`, `geometry-polygon`, `geometry-multipolygon`, `geometry-geometrycollection`, `geometry-any`.
- **XOgcPropertySeq**: Sequence number of the property (not yet used).

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
			"DateTimeColumn": "date"
		}
	},
	"SchemaOptions": {
		"Title": "collection title",
		"Description": "collection description",
		"Properties": {
			"name": {
				"Title": "Name",
				"Description": "decription of property",
				"Type": "string"
			},
			"number": {
				"Title": "Number"
			},
			"id": {
				"Title": "Id"
			},
			"date": {
				"Title": "Date"
			}
		}
	}
}
```
</details>

## Notes
- If the `SchemaOptions` section is not specified, property names and types will be retrieved from the data provider.
- If the `Features.Storage.Properties` list is defined, the generated schema will include only those properties, with `IdentifierColumn` used as the ID and `GeometryColumn` as the geometry.
- If a property does not have an explicit `Type` defined, the type will be inferred from the data source.
- The `format` and `x-ogc-role` must be explicitly set for geometry properties. If the format is not specified, it will be obtained: for mbtiles - it will be read from mbtiles metadata, for collections that have `Features.Storage.GeometryGeoJsonType` - from this field. If `x-ogc-role` is not provided, it will default to `primary-geometry` for geometry property.
- If the `IdentifierColumn` is set in `Features.Storage`, the corresponding property with the same name will be assigned the `x-ogc-role` value: `id`.

For more details about the Features API, see [Features API](features-api.md).