using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Features.PostGis.Tests.Utils;
using OgcApi.Net.Features.PostGis;
using System.Collections.Generic;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Features.PostGis.Tests
{
    public static class TestProviders
    {
        private static SqlCollectionSourcesOptions GetDefaultOptions()
        {
            return new SqlCollectionSourcesOptions
            {
                Sources = new List<SqlCollectionSourceOptions>
                {
                    new()
                    {
                        Id = "Polygons",
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
                    },
                    new()
                    {
                        Id = "LineStrings",
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
                    },
                    new()
                    {
                        Id = "Points",
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
                    },
                    new()
                    {
                        Id = "Empty",
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
                    },
                    new()
                    {
                        Id = "PolygonsForInsert",
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
            };
        }

        private static SqlCollectionSourcesOptions GetOptionsWithUnknownTable()
        {
            return new SqlCollectionSourcesOptions
            {
                Sources = new List<SqlCollectionSourceOptions>
                {
                    new()
                    {
                        Id = "Test",
                        ConnectionString = DatabaseUtils.GetConnectionString(),
                        Schema = "public",
                        Table = "test",
                        IdentifierColumn = "id",
                        GeometryColumn = "geom",
                        GeometryDataType = "geometry",
                        GeometrySrid = 3857
                    }
                }
            };
        }

        private static SqlCollectionSourcesOptions GetOptionsWithApiKey()
        {
            return new SqlCollectionSourcesOptions
            {
                Sources = new List<SqlCollectionSourceOptions>
                {
                    new()
                    {
                        Id = "PointsWithApiKey",
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
            };
        }

        public static PostGisProvider GetDefaultProvider()
        {
            var options = GetDefaultOptions();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new PostGisProvider(optionsMonitor, new NullLogger<PostGisProvider>());
        }

        public static PostGisProvider GetProviderWithErrors()
        {
            var options = GetOptionsWithUnknownTable();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new PostGisProvider(optionsMonitor, new NullLogger<PostGisProvider>());
        }

        public static PostGisProvider GetProviderWithApiKey()
        {
            var options = GetOptionsWithApiKey();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new PostGisProvider(optionsMonitor, new NullLogger<PostGisProvider>());
        }
    }
}
