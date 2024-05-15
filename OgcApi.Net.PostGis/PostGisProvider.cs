using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using System.Data.Common;

namespace OgcApi.Net.PostGis;

[OgcFeaturesProvider("PostGis", typeof(SqlFeaturesSourceOptions))]
[OgcTilesProvider("PostGis", null)]
public class PostGisProvider(ILogger<PostGisProvider> logger, IOptionsMonitor<OgcApiOptions> options)
    : SqlDataProvider(logger, options)
{
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