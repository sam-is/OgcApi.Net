# OgcApi.Net.SqlServer

SQL Server provider for OgcApi.Net. This package allows you to publish geospatial data from SQL Server databases through the OGC API.

## Installation
Install the package via NuGet:
```bash
dotnet add package OgcApi.Net.SqlServer
```

## Usage
To use this provider, register it in your `Startup.cs` file:
```csharp
services.AddOgcApiSqlServerProvider();
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
	  	    "Type": "SqlServer",
	  	    "ConnectionString": "Server=localhost;Database=Tests;User Id=sa;Password=myStrongP@ssword;",
	  	    "Schema": "dbo",
	  	    "Table": "TestTable",
	  	    "GeometryColumn": "Geom",
	  	    "GeometrySrid": 3857,
	  	    "GeometryDataType": "geometry",
	  	    "GeometryGeoJsonType": "MultiPolygon",
	  	    "IdentifierColumn": "Id",
	  	    "Properties": [
	  	  	  "Name",
	  	  	  "Region"
	  	    ]
	  	  }
	    }
	  }
    ]
  }
}
```

For more details, see the [full documentation](https://sam-is.github.io/OgcApi.Net/installation).

## License
This project is licensed under the MIT License. See the [LICENSE](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE) file for details.