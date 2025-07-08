using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Net.SqlServer.Tests.Utils;
using System;

namespace OgcApi.Net.SqlServer.Tests;

public static class TestProviders
{
    private static OgcApiOptions GetDefaultOptions()
    {
        return new OgcApiOptions
        {
            Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Id = "Polygons",
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "Polygons",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "Date",
                                Properties =
                                [
                                    "Name",
                                    "Number",
                                    "S",
                                    "Date"
                                ]
                            }
                        }
                    },

                    new CollectionOptions
                    {
                        Id = "LineStrings",
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "LineStrings",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["Name"]
                            }
                        }
                    },

                    new CollectionOptions
                    {
                        Id = "Points",
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "Points",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["Name"]
                            }
                        }
                    },

                    new CollectionOptions
                    {
                        Id = "Empty",
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "EmptyTable",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["Name"]
                            }
                        }
                    },

                    new CollectionOptions
                    {
                        Id = "PolygonsForInsert",
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "PolygonsForInsert",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "Date",
                                Properties =
                                [
                                    "Name",
                                    "Number",
                                    "S",
                                    "Date"
                                ]
                            }
                        }
                    }
                ]
            }
        };
    }

    private static OgcApiOptions GetOptionsWithUnknownTable()
    {
        return new OgcApiOptions
        {
            Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Id = "Test",
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "Test",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857
                            }

                        }

                    }
                ]
            }
        };
    }

    private static OgcApiOptions GetOptionsWithApiKey()
    {
        return new OgcApiOptions
        {
            Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Id = "PointsWithApiKey",
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "PointsWithApiKey",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["Name"],
                                ApiKeyPredicateForGet = "[Key] = @ApiKey"
                            }
                        }
                    }
                ]
            }
        };
    }

    public static SqlServerProvider GetDefaultProvider()
    {
        return new SqlServerProvider(new NullLogger<SqlServerProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetDefaultOptions()));
    }

    public static SqlServerProvider GetProviderWithErrors()
    {
        return new SqlServerProvider(new NullLogger<SqlServerProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetOptionsWithUnknownTable()));
    }

    public static SqlServerProvider GetProviderWithApiKey()
    {
        return new SqlServerProvider(new NullLogger<SqlServerProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetOptionsWithApiKey()));
    }
}