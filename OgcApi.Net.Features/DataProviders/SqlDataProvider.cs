using Microsoft.Extensions.Logging;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features.Features;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Options.Interfaces;
using OgcApi.Net.Features.Options.SqlOptions;
using System;
using System.Data.Common;
using System.Linq;
using System.Text.Json;

namespace OgcApi.Net.Features.DataProviders
{
    public abstract class SqlDataProvider : IDataProvider
    {
        public const int FeaturesMinimumLimit = 1;

        public const int FeaturesMaximumLimit = 10000;

        protected ICollectionsOptions CollectionsOptions;

        protected readonly ILogger Logger;

        public abstract string SourceType { get; }

        protected SqlDataProvider(ILogger logger)
        {
            Logger = logger;
        }

        public ICollectionsOptions GetCollectionSourcesOptions()
        {
            return CollectionsOptions;
        }

        public Envelope GetBbox(string collectionId, string apiKey = null)
        {
            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                using var selectBboxCommand = featuresQueryBuilder
                    .AddSelectBbox()
                    .BuildCommand(connection);

                using var reader = (DbDataReader)selectBboxCommand.ExecuteReader();
                reader.Read();
                if (reader.IsDBNull(0))
                    return null;

                var geometry = ReadGeometry(reader, 0, sourceOptions);

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
            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }
            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            if (!string.IsNullOrWhiteSpace(sourceOptions.ApiKeyPredicateForGet) && string.IsNullOrWhiteSpace(apiKey))
            {
                Logger.LogTrace("API key is not supplied");
                throw new UnauthorizedAccessException("API key is not supplied");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                var selectFeatureCommand = featuresQueryBuilder
                    .AddSelect()
                    .AddFrom()
                    .AddWhere(featureId)
                    .AddApiKeyWhere(sourceOptions.ApiKeyPredicateForGet, apiKey)
                    .ComposeWhereClause()
                    .BuildCommand(connection);

                using var reader = (DbDataReader)selectFeatureCommand.ExecuteReader();
                try
                {
                    if (reader.Read())
                        if (!reader.IsDBNull(1))
                        {
                            var geometry = ReadGeometry(reader, 1, sourceOptions);
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

            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }
            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            if (!string.IsNullOrWhiteSpace(sourceOptions.ApiKeyPredicateForGet) && string.IsNullOrWhiteSpace(apiKey))
            {
                Logger.LogTrace("API key is not supplied");
                throw new UnauthorizedAccessException("API key is not supplied");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                var selectFeaturesCommand = featuresQueryBuilder
                    .AddSelect()
                    .AddFrom()
                    .AddWhere(bbox)
                    .AddWhere(startDateTime, endDateTime)
                    .AddApiKeyWhere(sourceOptions.ApiKeyPredicateForGet, apiKey)
                    .ComposeWhereClause()
                    .AddLimit(offset, limit)
                    .BuildCommand(connection);

                Logger.LogTrace($"Query: {selectFeaturesCommand.CommandText}");

                using var reader = (DbDataReader)selectFeaturesCommand.ExecuteReader();
                var featureCollection = new OgcFeatureCollection();
                while (reader.Read())
                    if (!reader.IsDBNull(1))
                    {
                        var geometry = ReadGeometry(reader, 1, sourceOptions);

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
            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                var selectFeaturesCommand = featuresQueryBuilder
                    .AddCount()
                    .AddFrom()
                    .AddWhere(bbox)
                    .AddWhere(startDateTime, endDateTime)
                    .AddApiKeyWhere(sourceOptions.ApiKeyPredicateForGet, apiKey)
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

            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                var insertFeatureCommand = featuresQueryBuilder
                    .AddInsert(feature)
                    .AddApiKeyWhere(sourceOptions.ApiKeyPredicateForCreate, apiKey)
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

            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                var updateFeatureCommand = featuresQueryBuilder
                    .AddUpdate(feature)
                    .AddWhere(featureId)
                    .AddApiKeyWhere(sourceOptions.ApiKeyPredicateForUpdate, apiKey)
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

            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                var replaceFeatureCommand = featuresQueryBuilder
                    .AddReplace(feature)
                    .AddWhere(featureId)
                    .AddApiKeyWhere(sourceOptions.ApiKeyPredicateForUpdate, apiKey)
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

            var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
            if (collectionOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
            }

            var sourceOptions = (SqlCollectionSourceOptions)collectionOptions.Features?.Storage;
            if (sourceOptions == null)
            {
                Logger.LogTrace(
                    $"The source collection with ID = {collectionId} was found, yet it contains no storage options");
                throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
            }

            try
            {
                using var connection = GetDbConnection(sourceOptions.ConnectionString);
                connection.Open();

                var featuresQueryBuilder = GetFeaturesSqlQueryBuilder(sourceOptions);
                var deleteFeatureCommand = featuresQueryBuilder
                    .AddDelete()
                    .AddWhere(featureId)
                    .AddApiKeyWhere(sourceOptions.ApiKeyPredicateForDelete, apiKey)
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

        public ICollectionSourceOptions DeserializeCollectionSourceOptions(string json, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<SqlCollectionSourceOptions>(json, options);
        }

        public void SerializeStorageOptions(Utf8JsonWriter writer, ICollectionSourceOptions item)
        {
            if (item is SqlCollectionSourceOptions storage)
            {
                writer.WriteStartObject("Storage");
                writer.WriteString("Type", storage.Type);
                writer.WriteString("ConnectionString", storage.ConnectionString);
                writer.WriteString("Schema", storage.Schema);
                writer.WriteString("Table", storage.Table);
                writer.WriteString("GeometryColumn", storage.GeometryColumn);
                writer.WriteString("GeometryDataType", storage.GeometryDataType);
                writer.WriteString("GeometryGeoJsonType", storage.GeometryGeoJsonType);
                writer.WriteNumber("GeometrySrid", storage.GeometrySrid);
                writer.WriteString("DateTimeColumn", storage.DateTimeColumn);
                writer.WriteString("IdentifierColumn", storage.IdentifierColumn);
                if (storage.Properties != null && storage.Properties.Any())
                {
                    writer.WriteStartArray("Properties");
                    foreach (var prop in item.Properties)
                        writer.WriteStringValue(prop);
                    writer.WriteEndArray();
                }
                writer.WriteBoolean("AllowCreate", storage.AllowCreate);
                writer.WriteBoolean("AllowReplace", storage.AllowReplace);
                writer.WriteBoolean("AllowUpdate", storage.AllowUpdate);
                writer.WriteBoolean("AllowDelete", storage.AllowDelete);
                writer.WriteString("ApiKeyPredicateForGet", storage.ApiKeyPredicateForGet);
                writer.WriteString("ApiKeyPredicateForCreate", storage.ApiKeyPredicateForCreate);
                writer.WriteString("ApiKeyPredicateForUpdate", storage.ApiKeyPredicateForUpdate);
                writer.WriteString("ApiKeyPredicateForDelete", storage.ApiKeyPredicateForDelete);
                writer.WriteEndObject();
            }
        }
        public void SetCollectionsOptions(ICollectionsOptions options)
        {
            switch (options)
            {
                case null:
                    throw new ArgumentNullException(nameof(options));
                case CollectionsOptions collectionOptions:
                {
                    CollectionsOptionsValidator.Validate(collectionOptions);
                    if (collectionOptions.Items.Any(i => i.Features.Storage.Type == SourceType))
                    {
                        var resultingOptions = new CollectionsOptions
                        {
                            Items = collectionOptions.Items.Where(i => i.Features.Storage.Type == SourceType).ToList()
                        };
                        if (collectionOptions.Items != null) resultingOptions.Links = collectionOptions.Links;
                        CollectionsOptions = resultingOptions;
                    }

                    break;
                }
            }
        }
    }
}
