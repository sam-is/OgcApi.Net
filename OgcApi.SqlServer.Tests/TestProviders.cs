using Microsoft.Extensions.Logging.Abstractions;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Net.SqlServer;
using OgcApi.SqlServer.Tests.Utils;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;

namespace OgcApi.SqlServer.Tests
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
                            Id ="Polygons",
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
                                    Type = "SqlServer",
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
                }
            };
        }

        public static SqlServerProvider GetDefaultProvider()
        {
            return new SqlServerProvider(new NullLogger<SqlServerProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetDefaultOptions()));
        }

        public static SqlServerProvider GetProviderWithErrors()
        {
            return new SqlServerProvider(new NullLogger<SqlServerProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetOptionsWithUnknownTable()));
        }

        public static SqlServerProvider GetProviderWithApiKey()
        {
            return new SqlServerProvider(new NullLogger<SqlServerProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetOptionsWithApiKey()));
        }
    }
}
