using System;
using Microsoft.Extensions.Logging.Abstractions;
using OgcApi.Features.SqlServer.Tests.Utils;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.SqlServer;
using System.Collections.Generic;

namespace OgcApi.Features.SqlServer.Tests
{
    public static class TestProviders
    {
        private static CollectionsOptions GetDefaultOptions()
        {
            return new CollectionsOptions
            {
                Items = new List<CollectionOptions>
                {
                    new()
                    {
                        Id ="Polygons",
                        Features = new CollectionOptionsFeatures
                        {
                            Crs = new List<Uri>
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions
                            {
                                Type= "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "Polygons",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "Date",
                                Properties = new List<string>
                                {
                                    "Name",
                                    "Number",
                                    "S",
                                    "Date"
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id ="LineStrings",
                        Features = new CollectionOptionsFeatures
                        {
                            Crs = new List<Uri>
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions
                            {
                                Type= "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "LineStrings",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>
                                {
                                    "Name"
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id ="Points",
                        Features = new CollectionOptionsFeatures
                        {
                            Crs = new List<Uri>
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions
                            {
                                Type= "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "Points",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>
                                {
                                    "Name"
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id ="Empty",
                        Features = new CollectionOptionsFeatures
                        {
                            Crs = new List<Uri>
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions
                            {
                                Type= "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "EmptyTable",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>
                                {
                                    "Name"
                                }
                            }
                        }
                    },
                    new()
                    {
                        Id = "PolygonsForInsert",
                        Features = new CollectionOptionsFeatures
                        {
                            Crs = new List<Uri>
                            {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions
                            {
                                Type= "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "PolygonsForInsert",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                DateTimeColumn = "Date",
                                Properties = new List<string>
                                {
                                    "Name",
                                    "Number",
                                    "S",
                                    "Date"
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
                Items = new List<CollectionOptions>
                {
                    new()
                    {
                        Id ="Test",
                        Features = new CollectionOptionsFeatures
                        {
                             Crs = new List<Uri>
                             {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions
                            {
                                Type= "SqlServer",
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
                }
            };
        }

        private static CollectionsOptions GetOptionsWithApiKey()
        {
            return new CollectionsOptions
            {
                Items = new List<CollectionOptions>
                {
                    new()
                    {
                        Id = "PointsWithApiKey",
                        Features = new CollectionOptionsFeatures
                        {
                             Crs = new List<Uri>
                             {
                                new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new("http://www.opengis.net/def/crs/EPSG/0/3857")
                            },
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlCollectionSourceOptions
                            {
                                Type= "SqlServer",
                                ConnectionString = DatabaseUtils.GetConnectionString(),
                                Schema = "dbo",
                                Table = "PointsWithApiKey",
                                IdentifierColumn = "Id",
                                GeometryColumn = "Geom",
                                GeometryDataType = "geometry",
                                GeometrySrid = 3857,
                                Properties = new List<string>
                                {
                                    "Name"
                                },
                                ApiKeyPredicateForGet = "[Key] = @ApiKey"
                            }
                        }
                    }
                }
            };
        }

        public static SqlServerProvider GetDefaultProvider()
        {
            CollectionsOptions options = GetDefaultOptions();
            var provider = new SqlServerProvider(new NullLogger<SqlServerProvider>());
            provider.SetCollectionsOptions(options);
            return provider;
        }

        public static SqlServerProvider GetProviderWithErrors()
        {
            CollectionsOptions options = GetOptionsWithUnknownTable();
            var provider = new SqlServerProvider(new NullLogger<SqlServerProvider>());
            provider.SetCollectionsOptions(options);
            return provider;
        }

        public static SqlServerProvider GetProviderWithApiKey()
        {
            CollectionsOptions options = GetOptionsWithApiKey();
            var provider = new SqlServerProvider(new NullLogger<SqlServerProvider>());
            provider.SetCollectionsOptions(options);
            return provider;
        }
    }
}
