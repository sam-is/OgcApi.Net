# Coordinate Systems

The API supports any coordinate system identified by an SRID (Spatial Reference System Identifier). Each coordinate system must have a corresponding URI.

## Adding Custom Coordinate Systems
To add custom coordinate systems, modify the `SRID.csv` file provided by the NuGet package.

### Example of `SRID.csv` Format
```csv
3857;PROJCS["Popular Visualisation CRS / Mercator", GEOGCS["Popular Visualisation CRS", DATUM["WGS84", SPHEROID["WGS84", 6378137.0, 298.257223563, AUTHORITY["EPSG","7059"]], AUTHORITY["EPSG","6055"]], PRIMEM["Greenwich", 0, AUTHORITY["EPSG", "8901"]], UNIT["degree", 0.0174532925199433, AUTHORITY["EPSG", "9102"]], AXIS["E", EAST], AXIS["N", NORTH], AUTHORITY["EPSG","4055"]], PROJECTION["Mercator"], PARAMETER["semi_minor",6378137], PARAMETER["False_Easting", 0], PARAMETER["False_Northing", 0], PARAMETER["Central_Meridian", 0], PARAMETER["Latitude_of_origin", 0], UNIT["metre", 1, AUTHORITY["EPSG", "9001"]], AXIS["East", EAST], AXIS["North", NORTH], AUTHORITY["EPSG","3785"]]
4326;GEOGCS["WGS 84",DATUM["WGS_1984",SPHEROID["WGS 84",6378137,298.257223563,AUTHORITY["EPSG","7030"]],AUTHORITY["EPSG","6326"]],PRIMEM["Greenwich",0,AUTHORITY["EPSG","8901"]],UNIT["degree",0.01745329251994328,AUTHORITY["EPSG","9122"]],AUTHORITY["EPSG","4326"]]
```