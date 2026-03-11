---
layout: default
title: Overview
nav_order: 2
---

# Overview

OGC API standards define modular API building blocks to spatially enable Web APIs in a consistent way. The OpenAPI specification is used as the foundation for defining these building blocks.

Currently, this project implements the following standards:

| Standard | Data Providers |
| --- | --- |
| [OGC API - Features - Part 1: Core](http://www.opengis.net/doc/IS/ogcapi-features-1/1.0) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis <br> SQLite/SpatiaLite |
| [OGC API - Features - Part 2: Coordinate Reference Systems by Reference](http://www.opengis.net/doc/IS/ogcapi-features-2/1.0) | Independent |
| [OGC API - Features - Part 4: Create, Replace, Update and Delete](https://docs.ogc.org/DRAFTS/20-002r1.html) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis <br> SQLite/SpatiaLite |
| [OGC API - Features - Part 5: Schemas](https://portal.ogc.org/files/108199 ) | Microsoft SQL Server 2012+ <br> Azure SQL Database <br> PostgreSQL/PostGis <br> Sqlite/MbTiles |
| [OGC API - Tiles - Part 1: Core](https://docs.ogc.org/is/20-057/20-057.html) | Sqlite/MbTiles |
| [OGC API - Styles](https://docs.ogc.org/DRAFTS/20-009.html) | |

This project uses:
- ASP.NET Core 10 for building Web APIs
- [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite ) for feature representation
- [ProjNET](https://github.com/NetTopologySuite/ProjNet4GeoAPI ) for coordinate transformations
- [OpenAPI.NET](https://github.com/Microsoft/OpenAPI.NET ) for OpenAPI document generation

## OpenAPI.NET version

OpenAPI.NET v3.3.1 contains [breaking changes](https://github.com/microsoft/OpenAPI.NET/blob/main/docs/upgrade-guide-2.md) compared to v1.6. If your project uses OpenAPI.NET < 2.0.0, you must either:
- Upgrade to OpenAPI.NET ≥ 2.0.0, or
- Use OgcApi.Net v1.2.2 (which depends on OpenAPI.NET 1.6.14)

### **Compatibility with Microsoft.AspNetCore.OpenApi v10+**

`OpenAPI.NET` v3.3.1 is [not compatible](https://github.com/microsoft/OpenAPI.NET/blob/main/docs/upgrade-guide-3.md#integrations-with-aspnet-core) with `Microsoft.AspNetCore.OpenApi` v10+.

If your application uses `Microsoft.AspNetCore.OpenApi`, please use **OgcApi.Net v3.0.0**. This version depends on `OpenAPI.NET` v2.7.0, which maintains compatibility with `Microsoft.AspNetCore.OpenApi` v10+.

### Version Compatibility

| OgcApi.Net Version | OpenAPI.NET Dependency | Notes |
| :--- | :--- | :--- |
| **1.2.2** | 1.6.14 | Legacy version |
| **2.0.0** | 3.3.1 | Not compatible with `Microsoft.AspNetCore.OpenApi` v10+ |
| **3.0.0** | 2.7.0 | Compatible with `Microsoft.AspNetCore.OpenApi` v10+ |

The source code targets .NET 8 and .NET 10.

For more information about NuGet packages, see [Installation](installation.md).