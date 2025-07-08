using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Net.PostGis.Tests.Utils;
using System;

namespace OgcApi.Net.PostGis.Tests;

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
                                Type = "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "polygons",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "date",
                                Properties =
                                [
                                    "name",
                                    "num",
                                    "s",
                                    "date"
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
                                Type = "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "linestrings",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["name"]
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
                                Type = "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "points",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["name"]
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
                                Type = "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "empty_table",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["name"]
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
                                Type = "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "polygons_for_insert",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "date",
                                Properties =
                                [
                                    "name",
                                    "num",
                                    "s",
                                    "date"
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
                                Type = "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "test",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
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
                                Type = "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "points_with_api_key",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = ["name"],
                                ApiKeyPredicateForGet = "key = @ApiKey"
                            }
                        }
                    }
                ]
            }
        };
    }

    public static PostGisProvider GetDefaultProvider()
    {
        return new PostGisProvider(new NullLogger<PostGisProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetDefaultOptions()));
    }

    public static PostGisProvider GetProviderWithErrors()
    {
        return new PostGisProvider(new NullLogger<PostGisProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetOptionsWithUnknownTable()));
    }

    public static PostGisProvider GetProviderWithApiKey()
    {
        return new PostGisProvider(new NullLogger<PostGisProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetOptionsWithApiKey()));
    }
}