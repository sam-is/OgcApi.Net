using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using System.Data.Common;

namespace OgcApi.Net.SqlServer;

[OgcFeaturesProvider("SqlServer", typeof(SqlFeaturesSourceOptions))]
[OgcTilesProvider("SqlServer", null)]
public class SqlServerProvider(ILogger<SqlServerProvider> logger, IOptionsMonitor<OgcApiOptions> options)
    : SqlDataProvider(logger, options)
{
    protected override DbConnection GetDbConnection(string connectionString)
    {
        return new SqlConnection(connectionString);
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
            IsGeography = collectionSourceOptions.GeometryDataType == "geography"
        };
        return geometryReader.Read(geometryStream);
    }
}