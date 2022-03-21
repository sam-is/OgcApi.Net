using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.Json;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Options.Interfaces;

namespace OgcApi.Net.Features.SqlServer
{
    public class SqlServerProvider : SqlDataProvider
    {
        public SqlServerProvider(ILogger<SqlServerProvider> logger)
            : base(logger) {}
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
        public override ICollectionSourceOptions DeserializeCollectionSourceOptions(string json, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<SqlCollectionSourceOptions>(json, options);
        }

        public override void SetCollectionOptions(ICollectionsOptions options)
        {
            if (options is CollectionsOptions collectionOptions
                && collectionOptions?.Items != null
                && collectionOptions.Items.Any(i => i.Features.Storage.Type == SourceType))
            {
                CollectionsOptions = new CollectionsOptions()
                {
                    Links = collectionOptions.Links,
                    Items = collectionOptions.Items.Where(i => i.Features.Storage.Type == SourceType).ToList()
                };
            }
        }
    }
}