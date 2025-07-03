# OgcApi.Net

.NET implementation of the OGC API family of standards. This package provides the core functionality for building Web APIs that conform to OGC API standards.

## Features
- Implements OGC API - Features, Tiles, and Schemas standards.
- Supports modular API building blocks based on OpenAPI specifications.
- Provides automatic API generation from metadata descriptions.

## Installation
Install the package via NuGet:
```bash
dotnet add package OgcApi.Net
```

## Usage
To use this package, follow these steps:

1. **Register the API in `Startup.cs`:**
   ```csharp
   services.AddOgcApi("ogcapi.json");
   ```

2. **Add controllers:**
   ```csharp
   services.AddControllers().AddOgcApiControllers();
   ```

3. **Configure the API using a JSON file (`ogcapi.json`), data provider for example PostGis:**
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

4. **Run your application and access the API at `/api/ogc`.**

For more details, see the [full documentation](https://sam-is.github.io/OgcApi.Net).

## License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE) file for details.