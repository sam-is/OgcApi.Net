# OgcApi.Net
.Net implementation of the OGC API family of standards

[![GitHub](https://img.shields.io/github/license/sam-is/OgcApi.Net)](https://github.com/sam-is/OgcApi.Net/blob/main/LICENSE)

## Overview

OGC API standards define modular API building blocks to spatially enable Web APIs in a consistent way. The OpenAPI specification is used to define the API building blocks.

Currently this project implements the following standards:

Standard | Data Providers
--- | ---
[OGC API - Features - Part 1: Core](http://www.opengis.net/doc/IS/ogcapi-features-1/1.0) | Microsoft SQL Server 2012+ <br> Azure SQL Database
[OGC API - Features - Part 2: Coordinate Reference Systems by Reference](http://www.opengis.net/doc/IS/ogcapi-features-2/1.0) | Independent 

This project uses:
- ASP.NET Core 5.0 for building web API
- [NetTopologySuite](https://github.com/NetTopologySuite/NetTopologySuite) for the features representation
- [ProjNET](https://github.com/NetTopologySuite/ProjNet4GeoAPI) for the coordinate conversions

