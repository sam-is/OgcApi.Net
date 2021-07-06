using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Npgsql;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Features;
using OgcApi.Net.Features.PostGis.Options;
using System;

namespace OgcApi.Net.Features.PostGis
{
    public class PostGisProvider : IDataProvider
    {
        public const int FeaturesMinimumLimit = 1;

        public const int FeaturesMaximumLimit = 10000;

        private readonly PostGisCollectionSourcesOptions _collectionsOptions;

        private readonly ILogger _logger;

        public PostGisProvider(IOptionsMonitor<PostGisCollectionSourcesOptions> sqlServerCollectionSourcesOptions,
            ILogger<PostGisProvider> logger)
        {
            if (sqlServerCollectionSourcesOptions == null)
                throw new ArgumentNullException(nameof(sqlServerCollectionSourcesOptions));

            _logger = logger;

            try
            {
                _collectionsOptions = sqlServerCollectionSourcesOptions.CurrentValue;
                PostGisCollectionSourcesOptionsValidator.Validate(_collectionsOptions);
            }
            catch (OptionsValidationException ex)
            {
                foreach (var failure in ex.Failures) _logger.LogError(failure);
                throw;
            }
        }

        public string SourceType { get; } = "PostGis";

        public Envelope GetBbox(string collectionId, string apiKey = null)
        {
            var collectionOptions = _collectionsOptions.Sources.Find(x => x.Id == collectionId);
            if (collectionOptions == null)
            {
                _logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided PostGis options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            var bboxQueryTemplate =
                $"SELECT ST_Envelope({collectionOptions.GeometryColumn}) FROM \"{collectionOptions.Schema}\".\"{collectionOptions.Table}\"";

            _logger.LogTrace($"GetBbox database query: {bboxQueryTemplate}");

            try
            {
                using var connection = new NpgsqlConnection(collectionOptions.ConnectionString);
                connection.Open();

                using var selectBboxCommand = new NpgsqlCommand(bboxQueryTemplate, connection);

                using var reader = selectBboxCommand.ExecuteReader();
                reader.Read();

                if (reader.IsDBNull(0))
                    return null;

                var geometryReader = new PostGisReader()
                {
                    RepairRings = true
                };
                var geometryStream = reader.GetStream(0);

                _logger.LogTrace("GetBbox database query completed successfully");

                return geometryReader.Read(geometryStream).EnvelopeInternal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBbox database query completed with an exception");
                throw;
            }
        }

        public OgcFeature GetFeature(string collectionId, string featureId, string apiKey = null)
        {
            var collectionOptions = _collectionsOptions.Sources.Find(x => x.Id == collectionId);
            if (collectionOptions == null)
            {
                _logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided Sql Server options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = new NpgsqlConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = new FeaturesSqlQueryBuilder(collectionOptions);
                var selectFeatureCommand = featuresQueryBuilder
                    .AddSelect()
                    .AddFrom()
                    .AddWhere(featureId)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicate, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                using var reader = selectFeatureCommand.ExecuteReader();

                try
                {
                    if (reader.Read())
                        if (!reader.IsDBNull(1))
                        {
                            var geometryReader = new PostGisReader()
                            {
                                RepairRings = true
                            };
                            var geometryStream = reader.GetStream(0);
                            var geometry = geometryReader.Read(geometryStream);

                            var feature = new OgcFeature
                            {
                                Id = reader.GetValue(0).ToString(),
                                Geometry = geometry,
                                Attributes = new AttributesTable()
                            };
                            for (var i = 2; i < reader.FieldCount; i++)
                                if (!reader.IsDBNull(i))
                                    feature.Attributes.Add(reader.GetName(i), reader.GetValue(i));

                            return feature;
                        }
                }
                finally
                {
                    selectFeatureCommand.Dispose();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeature database query completed with an exception");
                throw;
            }
        }

        public OgcFeatureCollection GetFeatures(
            string collectionId,
            int limit = 10,
            int offset = 0,
            Envelope bbox = null,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            string apiKey = null)
        {
            if (limit < FeaturesMinimumLimit || limit > FeaturesMaximumLimit)
            {
                var errorMessage =
                    $"The limit parameter must be between {FeaturesMinimumLimit} and {FeaturesMaximumLimit}";
                _logger.LogError(errorMessage);
                throw new ArgumentOutOfRangeException(nameof(limit), errorMessage);
            }

            var collectionOptions = _collectionsOptions.Sources.Find(x => x.Id == collectionId);
            if (collectionOptions == null)
            {
                _logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided Sql Server options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            if (!string.IsNullOrWhiteSpace(collectionOptions.ApiKeyPredicate) && string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogTrace("API key is not supplied");
                throw new ArgumentException("API key is not supplied");
            }

            try
            {
                using var connection = new NpgsqlConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = new FeaturesSqlQueryBuilder(collectionOptions);
                var selectFeaturesCommand = featuresQueryBuilder
                    .AddSelect()
                    .AddFrom()
                    .AddWhere(bbox)
                    .AddWhere(startDateTime, endDateTime)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicate, apiKey)
                    .ComposeWhereClause()
                    .AddLimit(offset, limit)
                    .BuildCommand(connection);

                _logger.LogTrace($"Query: {selectFeaturesCommand.CommandText}");                

                using var reader = selectFeaturesCommand.ExecuteReader();
                var featureCollection = new OgcFeatureCollection();
                while (reader.Read())
                    if (!reader.IsDBNull(1))
                    {
                        var geometryReader = new PostGisReader()
                        {
                            RepairRings = true
                        };
                        var geometryStream = reader.GetStream(1);
                        var geometry = geometryReader.Read(geometryStream);

                        var feature = new OgcFeature
                        {
                            Id = reader.GetValue(0).ToString(),
                            Geometry = geometry,
                            Attributes = new AttributesTable()
                        };
                        for (var i = 2; i < reader.FieldCount; i++)
                            if (!reader.IsDBNull(i))
                                feature.Attributes.Add(reader.GetName(i), reader.GetValue(i));
                        featureCollection.Add(feature);
                    }

                selectFeaturesCommand.Dispose();

                return featureCollection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeatures database query completed with an exception");
                throw;
            }
        }

        public int GetFeaturesCount(
            string collectionId,
            Envelope bbox = null,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            string apiKey = null)
        {
            var collectionOptions = _collectionsOptions.Sources.Find(x => x.Id == collectionId);
            if (collectionOptions == null)
            {
                _logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided Sql Server options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = new NpgsqlConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = new FeaturesSqlQueryBuilder(collectionOptions);
                var selectFeaturesCommand = featuresQueryBuilder
                    .AddCount()
                    .AddFrom()
                    .AddWhere(bbox)
                    .AddWhere(startDateTime, endDateTime)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicate, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                var featuresCount = (long)selectFeaturesCommand.ExecuteScalar();
                selectFeaturesCommand.Dispose();

                return (int)featuresCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFeaturesCount database query completed with an exception");
                throw;
            }
        }
    }
}