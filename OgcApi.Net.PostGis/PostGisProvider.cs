using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Features;
using System.Data.Common;

namespace OgcApi.Net.PostGis
{
    public class PostGisProvider : SqlDataProvider
    {
        public PostGisProvider(ILogger<PostGisProvider> logger)
            : base(logger) { }

        public override string SourceType => "PostGis";

        protected override DbConnection GetDbConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }

        protected override IFeaturesSqlQueryBuilder GetFeaturesSqlQueryBuilder(SqlFeaturesSourceOptions collectionOptions)
        {
            return new FeaturesSqlQueryBuilder(collectionOptions);
        }

        protected override Geometry ReadGeometry(DbDataReader dataReader, int ordinal, SqlFeaturesSourceOptions collectionSourceOptions)
        {
            var geometryReader = new PostGisReader
            {
                RepairRings = false
            };

            return geometryReader.Read((byte[])dataReader.GetValue(ordinal));
        }
    }
}