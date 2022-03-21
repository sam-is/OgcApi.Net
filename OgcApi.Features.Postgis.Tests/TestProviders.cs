using Microsoft.Extensions.Logging.Abstractions;
using OgcApi.Features.PostGis.Tests.Utils;
using OgcApi.Net.Features.PostGis;
using System.Collections.Generic;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Options;

namespace OgcApi.Features.PostGis.Tests
{
    public static class TestProviders
    {
        private static CollectionsOptions GetDefaultOptions()
        {
            return new CollectionsOptions()
            {
                Items = new List<CollectionOptions>()
                {
                    new()
                    {
                        Id ="Polygons",
                        Features = new()
                        {
                            Crs = new()
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions()
                            {
                                Type= "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "polygons",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "date",
                                Properties = new List<string>()
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
                        Id ="LineStrings",
                        Features = new()
                        {
                            Crs = new()
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions()
                            {
                                Type= "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "linestrings",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>()
                                {
                                    "name"
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id ="Points",
                        Features = new()
                        {
                            Crs = new()
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions()
                            {
                                Type= "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "points",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>()
                                {
                                    "name"
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id ="Empty",
                        Features = new()
                        {
                            Crs = new()
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions()
                            {
                                Type= "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "empty_table",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>()
                                {
                                    "name"
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id = "PolygonsForInsert",
                        Features = new()
                        {
                            Crs = new()
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions()
                            {
                                Type= "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "polygons_for_insert",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "date",
                                Properties = new List<string>()
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
            };
        }
        private static CollectionsOptions GetOptionsWithUnknownTable()
        {
            return new CollectionsOptions
            {
                Items = new List<CollectionOptions>()
                {
                    new()
                    {
                        Id ="Test",
                        Features = new()
                        {
                             Crs = new()
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions()
                            {
                                Type= "PostGis",
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
            };
        }

        private static CollectionsOptions GetOptionsWithApiKey()
        {
            return new CollectionsOptions()
            {
                Items = new List<CollectionOptions>()
                {
                    new()
                    {
                        Id = "PointsWithApiKey",
                        Features = new()
                        {
                             Crs = new()
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions()
                            {
                                Type= "PostGis",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "public",
                                Table = "points_with_api_key",
                                IdentifierColumn = "id",
                                GeometryColumn = "geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>()
                                {
                                    "name"
                                },
                                ApiKeyPredicateForGet = "key = @ApiKey"
                            }
                        }
                    }
                }
            };
        }

        public static PostGisProvider GetDefaultProvider()
        {
            var options = GetDefaultOptions();
            var provider = new PostGisProvider(new NullLogger<PostGisProvider>());
            provider.SetCollectionOptions(options);
            return provider;
        }

        public static PostGisProvider GetProviderWithErrors()
        {
            var options = GetOptionsWithUnknownTable();
            var provider = new PostGisProvider(new NullLogger<PostGisProvider>());
            provider.SetCollectionOptions(options);
            return provider;
        }

        public static PostGisProvider GetProviderWithApiKey()
        {
            var options = GetOptionsWithApiKey();
            var provider = new PostGisProvider(new NullLogger<PostGisProvider>());
            provider.SetCollectionOptions(options);
            return provider;
        }
    }
}
