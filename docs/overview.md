# Overview

OGC API standards define modular API building blocks to spatially enable Web APIs in a consistent way. The OpenAPI specification is used as the foundation for defining these building blocks.

Currently, this project implements the following standards:

| Standard | Data Providers |
| --- | --- |
| [OGC API - Features - Part 1: Core](http://www.opengis.net/doc/IS/ogcapi-features-1/1.0) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis <br> SQLite/SpatiaLite |
| [OGC API - Features - Part 2: Coordinate Reference Systems by Reference](http://www.opengis.net/doc/IS/ogcapi-features-2/1.0) | Independent |
| [OGC API - Features - Part 4: Create, Replace, Update and Delete](http://docs.ogc.org/DRAFTS/20-002.html) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis <br> SQLite/SpatiaLite |
| [OGC API - Features - Part 5: Schemas](https://portal.ogc.org/files/108199 ) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis <br> Sqlite/MbTiles |
| [OGC API - Tiles - Part 1: Core](http://docs.ogc.org/DRAFTS/20-057.html) | Sqlite/MbTiles |

This project uses:
- ASP.NET Core 8 for building Web APIs
- [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite ) for feature representation
- [ProjNET](https://github.com/NetTopologySuite/ProjNet4GeoAPI ) for coordinate transformations
- [OpenAPI.NET](https://github.com/Microsoft/OpenAPI.NET ) for OpenAPI document generation

For more information about NuGet packages, see [Installation](installation.md).