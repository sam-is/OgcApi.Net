using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options;
using System.Data;
using System.Data.Common;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Options.Interfaces;
using System.Text.Json;

namespace OgcApi.Net.Features.SqlServer
{
    public class SqlServerProvider : SqlDataProvider
    {
        public SqlServerProvider(IOptionsMonitor<CollectionsOptions> sqlCollectionSourcesOptions, ILogger<SqlServerProvider> logger)
            : base(sqlCollectionSourcesOptions, logger)
        {
        }

        public override string SourceType => "SqlServer";

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        protected virtual IDbCommand GetDbCommand(string commandText, IDbConnection dbConnection)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }

        protected override IFeaturesSqlQueryBuilder GetFeaturesSqlQueryBuilder(SqlCollectionSourceOptions collectionOptions)
        {
            return new FeaturesSqlQueryBuilder(collectionOptions);
        }

        protected override Geometry ReadGeometry(DbDataReader dataReader, int ordinal, SqlCollectionSourceOptions collectionSourceOptions)
        {
            var geometryStream = dataReader.GetStream(ordinal);
            var geometryReader = new SqlServerBytesReader
            {
                RepairRings = true,
                IsGeography = collectionSourceOptions.GeometryDataType == "geography"
            };
            return geometryReader.Read(geometryStream);
        }

        public static ICollectionSourceOptions CastToCollectionSourceOptions(string element, JsonSerializerOptions options)
        {
            var collectionOptions = JsonSerializer.Deserialize<SqlCollectionSourceOptions>(element, options);
            if (collectionOptions != null && collectionOptions.Type == "SqlServer")
                return collectionOptions;
            else
                return null;

        }
    }
}