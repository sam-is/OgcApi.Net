using System.Collections.Generic;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Features.SqlServer.Tests.Utils;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.SqlServer;

namespace OgcApi.Features.SqlServer.Tests
{
    public static class TestProviders
    {
        public static SqlCollectionSourcesOptions GetDefaultOptions()
        {
            return new SqlCollectionSourcesOptions()
            {
                Sources = new List<SqlCollectionSourceOptions>()
                {
                    new()
                    {
                        Id = "Polygons",
                        ConnectionString = DatabaseUtils.GetConnectionString(),
                        Schema = "dbo",
                        Table = "Polygons",
                        IdentifierColumn = "Id",
                        GeometryColumn = "Geom",
                        GeometryDataType = "geometry",
                        GeometrySrid = 3857,
                        DateTimeColumn = "Date",
                        Properties = new List<string>()
                        {
                            "Name",
                            "Number",
                            "S",
                            "Date"
                        }
                    },
                    new()
                    {
                        Id = "LineStrings",
                        ConnectionString = DatabaseUtils.GetConnectionString(),
                        Schema = "dbo",
                        Table = "LineStrings",
                        IdentifierColumn = "Id",
                        GeometryColumn = "Geom",
                        GeometryDataType = "geometry",
                        GeometrySrid = 3857,
                        Properties = new List<string>()
                        {
                            "Name"
                        }
                    },
                    new()
                    {
                        Id = "Points",
                        ConnectionString = DatabaseUtils.GetConnectionString(),
                        Schema = "dbo",
                        Table = "Points",
                        IdentifierColumn = "Id",
                        GeometryColumn = "Geom",
                        GeometryDataType = "geometry",
                        GeometrySrid = 3857,
                        Properties = new List<string>()
                        {
                            "Name"
                        }
                    },
                    new()
                    {
                        Id = "Empty",
                        ConnectionString = DatabaseUtils.GetConnectionString(),
                        Schema = "dbo",
                        Table = "EmptyTable",
                        IdentifierColumn = "Id",
                        GeometryColumn = "Geom",
                        GeometryDataType = "geometry",
                        GeometrySrid = 3857,
                        Properties = new List<string>()
                        {
                            "Name"
                        }
                    }
                }
            };
        }

        public static SqlCollectionSourcesOptions GetOptionsWithUnknownTable()
        {
            return new SqlCollectionSourcesOptions()
            {
                Sources = new List<SqlCollectionSourceOptions>()
                {
                    new()
                    {
                        Id = "Test",
                        ConnectionString = DatabaseUtils.GetConnectionString(),
                        Schema = "dbo",
                        Table = "Test",
                        IdentifierColumn = "Id",
                        GeometryColumn = "Geom",
                        GeometryDataType = "geometry",
                        GeometrySrid = 3857
                    }
                }
            };
        }

        public static SqlCollectionSourcesOptions GetOptionsWithApiKey()
        {
            return new SqlCollectionSourcesOptions()
            {
                Sources = new List<SqlCollectionSourceOptions>()
                {
                    new()
                    {
                        Id = "PointsWithApiKey",
                        ConnectionString = DatabaseUtils.GetConnectionString(),
                        Schema = "dbo",
                        Table = "PointsWithApiKey",
                        IdentifierColumn = "Id",
                        GeometryColumn = "Geom",
                        GeometryDataType = "geometry",
                        GeometrySrid = 3857,
                        Properties = new List<string>()
                        {
                            "Name"
                        },
                        ApiKeyPredicateForGet = "[Key] = @ApiKey"
                    }
                }
            };
        }

        public static SqlServerProvider GetDefaultProvider()
        {
            SqlCollectionSourcesOptions options = GetDefaultOptions();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new SqlServerProvider(optionsMonitor, new NullLogger<SqlServerProvider>());
        }

        public static SqlServerProvider GetProviderWithErrors()
        {
            SqlCollectionSourcesOptions options = GetOptionsWithUnknownTable();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new SqlServerProvider(optionsMonitor, new NullLogger<SqlServerProvider>());
        }

        public static SqlServerProvider GetProviderWithApiKey()
        {
            SqlCollectionSourcesOptions options = GetOptionsWithApiKey();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new SqlServerProvider(optionsMonitor, new NullLogger<SqlServerProvider>());
        }
    }
}
