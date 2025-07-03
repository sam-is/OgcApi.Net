# OgcApi.Net.Schemas

Schemas support for OgcApi.Net. This package implements the OGC API - Features - Part 5: Schemas standard.

## Installation
Install the package via NuGet:
```bash
dotnet add package OgcApi.Net.Schemas
```

## Usage
To enable schema support, register the extension in your `Startup.cs` file **before** calling `AddOgcApi()`:
```csharp
services.AddSchemasOpenApiExtension();
services.AddOgcApi("ogcapi.json");
```

### Configuration Example
Add the following configuration to your `ogcapi.json` file:
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
	    "Id": "TestCollection",
	    "Title": "Test Collection",
	    "Features": {
	  	  "Crs": [
	  	    "http://www.opengis.net/def/crs/OGC/1.3/CRS84",
	  	    "http://www.opengis.net/def/crs/EPSG/0/3857"
	  	  ],
	  	  "StorageCrs": "http://www.opengis.net/def/crs/EPSG/0/3857",
	  	  "Storage": {
	  	    "Type": "PostGis",
	  	    "ConnectionString": "Host=localhost;User Id=postgre;Password=myStrongP@ssword;Database=Tests;Port=5432;",
	  	    "Schema": "public",
	  	    "Table": "test_table",
	  	    "GeometryColumn": "geom",
	  	    "GeometrySrid": 3857,
	  	    "GeometryDataType": "geometry",
	  	    "GeometryGeoJsonType": "MultiPolygon",
	  	    "IdentifierColumn": "id",
	  	    "Properties": [
	  	  	  "name",
	  	  	  "number"
	  	    ]
	  	  }
	    },
	    "SchemaOptions": {
	  	  "Title": "Collection Title",
	  	  "Description": "Collection Description",
	  	  "Properties": {
	  	    "name": {
	  	  	  "Title": "Name",
	  	  	  "Description": "Description of property",
	  	  	  "Type": "string"
	  	    },
	  	    "id": {
	  	  	  "Title": "Id"
	  	    },
	  	    "number": {
	  	  	  "Title": "Number"
	  	    }
	  	  }
	    }
	  }
    ]
  }
}
```

For more details, see the [full documentation](https://sam-is.github.io/OgcApi.Net/schemas).

## License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE) file for details.