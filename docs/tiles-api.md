# Tiles API

The current implementation supports only **MapBox Vector Tiles** for publishing through the API. Vector tiles must be stored in the **MbTiles** format. You can generate tiles from GeoJson files using [tippecanoe](https://github.com/mapbox/tippecanoe).

You can add the Tiles API to an existing collection or create a new collection that contains only tiles without the Features API.

## Tiles API Options
The Tiles API configuration includes the following options:
- **Crs**: Coordinate system used to store tiles.
- **Type**: Currently, only `MbTiles` is supported.
- **FileName**: Path to the MbTiles file.

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