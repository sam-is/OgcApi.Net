using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features.Features;
using OgcApi.Net.Features.Options;
using System;
using System.Data.Common;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Net.Features.DataProviders
{
    public abstract class SqlDataProvider : IDataProvider
    {
        public const int FeaturesMinimumLimit = 1;

        public const int FeaturesMaximumLimit = 10000;

        protected readonly SqlCollectionSourcesOptions CollectionsOptions;

        protected readonly ILogger Logger;

        public abstract string SourceType { get; }

        protected SqlDataProvider(IOptionsMonitor<SqlCollectionSourcesOptions> sqlCollectionSourcesOptions, ILogger logger)
        {
            if (sqlCollectionSourcesOptions == null)
                throw new ArgumentNullException(nameof(sqlCollectionSourcesOptions));

            Logger = logger;

            try
            {
                CollectionsOptions = sqlCollectionSourcesOptions.CurrentValue;
                SqlCollectionSourcesOptionsValidator.Validate(CollectionsOptions);
            }
            catch (OptionsValidationException ex)
            {
                foreach (var failure in ex.Failures) Logger.LogError(failure);
                throw;
            }
        }

        public ICollectionSourcesOptions GetCollectionSourcesOptions()
        {
            return CollectionsOptions;
        }

        public Envelope GetBbox(string collectionId, string apiKey = null)
        {
            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                using var selectBboxCommand = featuresQueryBuilder
                    .AddSelectBbox()
                    .BuildCommand(connection);

                using var reader = (DbDataReader)selectBboxCommand.ExecuteReader();
                reader.Read();
                if (reader.IsDBNull(0))
                    return null;

                var geometry = ReadGeometry(reader, 0, collectionOptions);

                Logger.LogTrace("GetBbox database query completed successfully");

                return geometry.EnvelopeInternal;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GetBbox database query completed with an exception");
                throw;
            }
        }

        public OgcFeature GetFeature(string collectionId, string featureId, string apiKey = null)
        {
            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            if (!string.IsNullOrWhiteSpace(collectionOptions.ApiKeyPredicateForGet) && string.IsNullOrWhiteSpace(apiKey))
            {
                Logger.LogTrace("API key is not supplied");
                throw new UnauthorizedAccessException("API key is not supplied");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                var selectFeatureCommand = featuresQueryBuilder
                    .AddSelect()
                    .AddFrom()
                    .AddWhere(featureId)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicateForGet, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                using var reader = (DbDataReader)selectFeatureCommand.ExecuteReader();
                try
                {
                    if (reader.Read())
                        if (!reader.IsDBNull(1))
                        {
                            var geometry = ReadGeometry(reader, 1, collectionOptions);
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
                Logger.LogError(ex, "GetFeature database query completed with an exception");
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
            if (limit is < FeaturesMinimumLimit or > FeaturesMaximumLimit)
            {
                var errorMessage =
                    $"The limit parameter must be between {FeaturesMinimumLimit} and {FeaturesMaximumLimit}";
                Logger.LogError(errorMessage);
                throw new ArgumentOutOfRangeException(nameof(limit), errorMessage);
            }

            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            if (!string.IsNullOrWhiteSpace(collectionOptions.ApiKeyPredicateForGet) && string.IsNullOrWhiteSpace(apiKey))
            {
                Logger.LogTrace("API key is not supplied");
                throw new UnauthorizedAccessException("API key is not supplied");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                var selectFeaturesCommand = featuresQueryBuilder
                    .AddSelect()
                    .AddFrom()
                    .AddWhere(bbox)
                    .AddWhere(startDateTime, endDateTime)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicateForGet, apiKey)
                    .ComposeWhereClause()
                    .AddLimit(offset, limit)
                    .BuildCommand(connection);

                Logger.LogTrace($"Query: {selectFeaturesCommand.CommandText}");

                using var reader = (DbDataReader)selectFeaturesCommand.ExecuteReader();
                var featureCollection = new OgcFeatureCollection();
                while (reader.Read())
                    if (!reader.IsDBNull(1))
                    {
                        var geometry = ReadGeometry(reader, 1, collectionOptions);

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
                Logger.LogError(ex, "GetFeatures database query completed with an exception");
                throw;
            }
        }

        public long GetFeaturesCount(
            string collectionId,
            Envelope bbox = null,
            DateTime? startDateTime = null,
            DateTime? endDateTime = null,
            string apiKey = null)
        {
            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                var selectFeaturesCommand = featuresQueryBuilder
                    .AddCount()
                    .AddFrom()
                    .AddWhere(bbox)
                    .AddWhere(startDateTime, endDateTime)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicateForGet, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                var featuresCount = Convert.ToInt64(selectFeaturesCommand.ExecuteScalar());
                selectFeaturesCommand.Dispose();

                return featuresCount;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GetFeaturesCount database query completed with an exception");
                throw;
            }
        }

        public string CreateFeature(string collectionId, IFeature feature, string apiKey = null)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (feature.Geometry == null)
            {
                throw new ArgumentException("Feature geometry cannot be null");
            }

            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                var insertFeatureCommand = featuresQueryBuilder
                    .AddInsert(feature)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicateForCreate, apiKey)
                    .BuildCommand(connection);

                var featureId = insertFeatureCommand.ExecuteScalar()?.ToString();
                insertFeatureCommand.Dispose();

                return featureId;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "CreateFeature database query completed with an exception");
                throw;
            }
        }

        public void UpdateFeature(string collectionId, string featureId, IFeature feature, string apiKey = null)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                var updateFeatureCommand = featuresQueryBuilder
                    .AddUpdate(feature)
                    .AddWhere(featureId)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicateForUpdate, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                var rowsAffected = updateFeatureCommand.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new ArgumentException($"Feature with ID = {featureId} does not exists");
                }
                updateFeatureCommand.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ReplaceFeature database query completed with an exception");
                throw;
            }
        }

        public void ReplaceFeature(string collectionId, string featureId, IFeature feature, string apiKey = null)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (feature.Geometry == null)
            {
                throw new ArgumentException("Feature geometry cannot be null");
            }

            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                var replaceFeatureCommand = featuresQueryBuilder
                    .AddReplace(feature)
                    .AddWhere(featureId)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicateForUpdate, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                var rowsAffected = replaceFeatureCommand.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new ArgumentException($"Feature with ID = {featureId} does not exists");
                }

                replaceFeatureCommand.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ReplaceFeature database query completed with an exception");
                throw;
            }
        }

        public void DeleteFeature(string collectionId, string featureId, string apiKey = null)
        {
            if (string.IsNullOrWhiteSpace(featureId))
            {
                throw new ArgumentNullException(nameof(featureId));
            }

            var collectionOptions = (SqlCollectionSourceOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(collectionOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(collectionOptions);
                var deleteFeatureCommand = featuresQueryBuilder
                    .AddDelete()
                    .AddWhere(featureId)
                    .AddApiKeyWhere(collectionOptions.ApiKeyPredicateForDelete, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                var rowsAffected = deleteFeatureCommand.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new ArgumentException($"Feature with ID = {featureId} does not exists");
                }

                deleteFeatureCommand.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "ReplaceFeature database query completed with an exception");
                throw;
            }
        }

        protected abstract DbConnection GetDbConnection(string connectionString);

        protected abstract IFeaturesSqlQueryBuilder GetFeaturesSqlQueryBuilder(SqlCollectionSourceOptions collectionOptions);

        protected abstract Geometry ReadGeometry(DbDataReader dataReader, int ordinal, SqlCollectionSourceOptions collectionSourceOptions);
    }
}
