using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Features.Options.Features;
using System.Data;
using System.Data.Common;

namespace OgcApi.Net.SqlServer
{
    public class SqlServerProvider : SqlDataProvider
    {
        public SqlServerProvider(ILogger<SqlServerProvider> logger)
            : base(logger) { }
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

        protected override IFeaturesSqlQueryBuilder GetFeaturesSqlQueryBuilder(SqlFeaturesSourceOptions collectionOptions)
        {
            return new FeaturesSqlQueryBuilder(collectionOptions);
        }

        protected override Geometry ReadGeometry(DbDataReader dataReader, int ordinal, SqlFeaturesSourceOptions collectionSourceOptions)
        {
            var geometryStream = dataReader.GetStream(ordinal);
            var geometryReader = new SqlServerBytesReader
            {
                RepairRings = true,
                IsGeography = collectionSourceOptions.GeometryDataType == "geography"
            };
            return geometryReader.Read(geometryStream);
        }
    }
}