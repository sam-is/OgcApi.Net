using Microsoft.Extensions.Logging.Abstractions;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Net.PostGis;
using OgcApi.PostGis.Tests.Utils;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;

namespace OgcApi.PostGis.Tests
{
    public static class TestProviders
    {
        private static OgcApiOptions GetDefaultOptions()
        {
            return new OgcApiOptions
            {
                Collections = new CollectionsOptions
                {
                    Items = new List<CollectionOptions>
                    {
                        new()
                        {
                            Id = "Polygons",
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857")
                                },
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
                                    Properties = new List<string>
                                    {
                                        "name",
                                        "num",
                                        "s",
                                        "date"
                                    }
                                }
                            }
                        },
                        new()
                        {
                            Id = "LineStrings",
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857")
                                },
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
                                    Properties = new List<string>
                                    {
                                        "name"
                                    }
                                }
                            }
                        },
                        new()
                        {
                            Id = "Points",
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857")
                                },
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
                                    Properties = new List<string>
                                    {
                                        "name"
                                    }
                                }
                            }
                        },
                        new()
                        {
                            Id = "Empty",
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857")
                                },
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
                                    Properties = new List<string>
                                    {
                                        "name"
                                    }
                                }
                            }
                        },
                        new()
                        {
                            Id = "PolygonsForInsert",
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857")
                                },
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
                                    Properties = new List<string>
                                    {
                                        "name",
                                        "num",
                                        "s",
                                        "date"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        private static OgcApiOptions GetOptionsWithUnknownTable()
        {
            return new OgcApiOptions
            {
                Collections = new CollectionsOptions
                {
                    Items = new List<CollectionOptions>
                    {
                        new()
                        {
                            Id = "Test",
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857")
                                },
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
                    }
                }
            };
        }

        private static OgcApiOptions GetOptionsWithApiKey()
        {
            return new OgcApiOptions
            {
                Collections = new CollectionsOptions
                {
                    Items = new List<CollectionOptions>
                    {
                        new()
                        {
                            Id = "PointsWithApiKey",
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857")
                                },
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
                                    Properties = new List<string>
                                    {
                                        "name"
                                    },
                                    ApiKeyPredicateForGet = "key = @ApiKey"
                                }
                            }
                        }
                    }
                }
            };
        }

        public static PostGisProvider GetDefaultProvider()
        {
            return new PostGisProvider(new NullLogger<PostGisProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetDefaultOptions()));
        }

        public static PostGisProvider GetProviderWithErrors()
        {
            return new PostGisProvider(new NullLogger<PostGisProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetOptionsWithUnknownTable()));
        }

        public static PostGisProvider GetProviderWithApiKey()
        {
            return new PostGisProvider(new NullLogger<PostGisProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetOptionsWithApiKey()));
        }
    }
}
