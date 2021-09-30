using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using OgcApi.Net.Features.DataProviders;
using System.Data;
using System.Data.Common;
using OgcApi.Net.Features.Options;

namespace OgcApi.Net.Features.PostGis
{
    public class PostGisProvider : SqlDataProvider
    {
        public PostGisProvider(IOptionsMonitor<SqlCollectionSourcesOptions> sqlCollectionSourcesOptions, ILogger logger)
            : base(sqlCollectionSourcesOptions, logger)
        {
        }

        public override string SourceType { get; } = "PostGis";

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
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
            var geometryReader = new PostGisReader()
            {
                RepairRings = true
            };
            var geometryStream = dataReader.GetStream(ordinal);
            return geometryReader.Read(geometryStream);
        }
    }
}