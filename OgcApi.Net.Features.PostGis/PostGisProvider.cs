using Npgsql;
using System.Data.Common;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using Microsoft.Extensions.Logging;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Net.Features.PostGis
{
    public class PostGisProvider : SqlDataProvider
    {
        public PostGisProvider(ILogger<PostGisProvider> logger)
            : base(logger) {}

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