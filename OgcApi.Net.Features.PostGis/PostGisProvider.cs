using Npgsql;
using System.Linq;
using System.Text.Json;
using System.Data.Common;
using NetTopologySuite.IO;
using NetTopologySuite.Geometries;
using Microsoft.Extensions.Logging;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Options.Interfaces;
using System;

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
        public override ICollectionSourceOptions DeserializeCollectionSourceOptions(string json, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<SqlCollectionSourceOptions>(json, options);
        }

        public override void SetCollectionOptions(ICollectionsOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            CollectionsOptionsValidator.Validate(options as CollectionsOptions);
            if (options is CollectionsOptions collectionOptions
                && collectionOptions?.Items != null
                && collectionOptions.Items.Any(i => i.Features.Storage.Type == SourceType))
            {
                CollectionsOptions = new CollectionsOptions();
                (CollectionsOptions as CollectionsOptions).Items = collectionOptions.Items.Where(i => i.Features.Storage.Type == SourceType).ToList();
                if (collectionOptions.Items != null) (CollectionsOptions as CollectionsOptions).Links = collectionOptions.Links;
            }
        }
    }
}