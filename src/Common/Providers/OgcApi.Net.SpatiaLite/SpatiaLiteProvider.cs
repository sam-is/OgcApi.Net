using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using System.Data.Common;

namespace OgcApi.Net.SpatiaLite;

[OgcFeaturesProvider("SpatiaLite", typeof(SqlFeaturesSourceOptions))]
[OgcTilesProvider("SpatiaLite", null)]
public class SpatiaLiteProvider(ILogger<SpatiaLiteProvider> logger, IOptionsMonitor<OgcApiOptions> options)
    : SqlDataProvider(logger, options)
{
    protected override DbConnection GetDbConnection(string connectionString)
    {
        return new SpatiaLiteConnection(connectionString);
    }

    protected override IFeaturesSqlQueryBuilder GetFeaturesSqlQueryBuilder(SqlFeaturesSourceOptions collectionOptions)
    {
        return new FeaturesSqlQueryBuilder(collectionOptions);
    }

    protected override Geometry ReadGeometry(DbDataReader dataReader, int ordinal, SqlFeaturesSourceOptions collectionSourceOptions)
    {
        var geometryStream = dataReader.GetStream(ordinal);

        var geometryReader = new GaiaGeoReader();
        return geometryReader.Read(geometryStream);
    }
}