using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Features.SqlServer.Tests.Utils;
using OgcApi.Net.Features.SqlServer;
using OgcApi.Net.Features.SqlServer.Options;
using System.Collections.Generic;

namespace OgcApi.Features.SqlServer.Tests
{
    public static class TestProviders
    {
        public static SqlServerCollectionSourcesOptions GetDefaultOptions()
        {
            return new SqlServerCollectionSourcesOptions()
            {
                Sources = new List<SqlServerCollectionSourceOptions>()
                {
                    new SqlServerCollectionSourceOptions()
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
                    new SqlServerCollectionSourceOptions()
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
                    new SqlServerCollectionSourceOptions()
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
                    new SqlServerCollectionSourceOptions()
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

        public static SqlServerCollectionSourcesOptions GetOptionsWithUnknownTable()
        {
            return new SqlServerCollectionSourcesOptions()
            {
                Sources = new List<SqlServerCollectionSourceOptions>()
                {
                    new SqlServerCollectionSourceOptions()
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

        public static SqlServerCollectionSourcesOptions GetOptionsWithApiKey()
        {
            return new SqlServerCollectionSourcesOptions()
            {
                Sources = new List<SqlServerCollectionSourceOptions>()
                {
                    new SqlServerCollectionSourceOptions()
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
                        ApiKeyPredicate = "[Key] = @ApiKey"
                    }
                }
            };
        }

        public static SqlServerProvider GetDefaultProvider()
        {
            SqlServerCollectionSourcesOptions options = GetDefaultOptions();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlServerCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new SqlServerProvider(optionsMonitor, new NullLogger<SqlServerProvider>());
        }

        public static SqlServerProvider GetProviderWithErrors()
        {
            SqlServerCollectionSourcesOptions options = GetOptionsWithUnknownTable();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlServerCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new SqlServerProvider(optionsMonitor, new NullLogger<SqlServerProvider>());
        }

        public static SqlServerProvider GetProviderWithApiKey()
        {
            SqlServerCollectionSourcesOptions options = GetOptionsWithApiKey();
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlServerCollectionSourcesOptions>>(mock => mock.CurrentValue == options);
            return new SqlServerProvider(optionsMonitor, new NullLogger<SqlServerProvider>());
        }
    }
}
