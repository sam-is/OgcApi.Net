using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using OgcApi.Net.DataProviders;
using System.Data.Common;
using OgcApi.Net.Options.SqlOptions;

namespace OgcApi.Net.PostGis
{
    public class PostGisProvider : SqlDataProvider
    {
        public PostGisProvider(IOptionsMonitor<SqlCollectionSourcesOptions> sqlCollectionSourcesOptions, ILogger<PostGisProvider> logger)
            : base(sqlCollectionSourcesOptions, logger)
        {
        }

        public override string SourceType => "PostGis";

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        protected override IFeaturesSqlQueryBuilder GetFeaturesSqlQueryBuilder(SqlCollectionSourceOptions collectionOptions)
        {
            return new FeaturesSqlQueryBuilder(collectionOptions);
        }

        protected override Geometry ReadGeometry(DbDataReader dataReader, int ordinal, SqlCollectionSourceOptions collectionSourceOptions)
        {
            var geometryReader = new PostGisReader()
            {
                RepairRings = false
            };

            return geometryReader.Read((byte[])dataReader.GetValue(ordinal));
        }
    }
}