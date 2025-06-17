using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.VectorTiles;
using NetTopologySuite.IO.VectorTiles.Mapbox;
using OgcApi.Net.Crs;
using OgcApi.Net.Features;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Net.Options.Interfaces;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace OgcApi.Net.DataProviders;

public abstract class SqlDataProvider(ILogger logger, IOptionsMonitor<OgcApiOptions> options)
    : IFeaturesProvider, ITilesProvider, IPropertyMetadataProvider
{
    public const int FeaturesMinimumLimit = 1;

    public const int FeaturesMaximumLimit = 10000;

    protected readonly ILogger Logger = logger;

    protected readonly ICollectionsOptions CollectionsOptions = options.CurrentValue.Collections;

    public Envelope GetBbox(string collectionId, string apiKey = null)
    {
        var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
        if (collectionOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }

        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
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
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }
        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
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
        string apiKey = null,
        Dictionary<string, string> propertyFilter = null)
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
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }
        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
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
                .AddWhere(propertyFilter)
                .ComposeWhereClause()
                .AddLimit(offset, limit)
                .BuildCommand(connection);

            Logger.LogTrace("Query: {query}", selectFeaturesCommand.CommandText);

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
        string apiKey = null,
        Dictionary<string, string> propertyFilter = null)
    {
        var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
        if (collectionOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }

        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
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
                .AddWhere(propertyFilter)
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
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }

        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
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
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }

        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
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
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }

        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
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

        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
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

    protected abstract IFeaturesSqlQueryBuilder GetFeaturesSqlQueryBuilder(SqlFeaturesSourceOptions collectionOptions);

    protected abstract Geometry ReadGeometry(DbDataReader dataReader, int ordinal, SqlFeaturesSourceOptions collectionSourceOptions);

    public Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileRow, int tileCol, string datetime = null, string apiKey = null)
    {
        var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
        if (collectionOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }

        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} has no storage options");
        }

        var bbox = CoordinateConverter.TileBounds(tileCol, tileRow, tileMatrix);
        bbox.Transform(CrsUtils.DefaultCrs, collectionOptions.Features.StorageCrs);

        var features = GetFeatures(collectionId, bbox: bbox, limit: 1000);
        var layer = new Layer { Name = collectionId };

        var bboxPolygon = new Polygon(
            new LinearRing(
                [
                    new Coordinate(bbox.MinX, bbox.MinY),
                    new Coordinate(bbox.MinX, bbox.MaxY),
                    new Coordinate(bbox.MaxX, bbox.MaxY),
                    new Coordinate(bbox.MaxX, bbox.MinY),
                    new Coordinate(bbox.MinX, bbox.MinY)
                ]
            )
        );

        var featureCollection = new OgcFeatureCollection();

        foreach (var feature in features)
        {
            if (feature.Geometry.OgcGeometryType != OgcGeometryType.Point)
            {
                var intersectedFeature = feature.Geometry.Intersection(bboxPolygon.Copy());
                if (intersectedFeature.IsEmpty)
                    continue;
                feature.Geometry = intersectedFeature;
            }
            featureCollection.Add(feature);
        }

        featureCollection.Transform(collectionOptions.Features.StorageCrs, CrsUtils.DefaultCrs);

        foreach (var feature in featureCollection)
            layer.Features.Add(feature);

        var vectorTile = new VectorTile { TileId = new NetTopologySuite.IO.VectorTiles.Tiles.Tile(tileCol, tileRow, tileMatrix).Id };
        vectorTile.Layers.Add(layer);

        using var compressedStream = new MemoryStream();
        using var compressor = new GZipStream(compressedStream, CompressionMode.Compress, true);

        vectorTile.Write(compressor, idAttributeName: sourceOptions.IdentifierColumn);
        compressor.Flush();

        compressedStream.Seek(0, SeekOrigin.Begin);

        return Task.FromResult(compressedStream.ToArray());
    }

    public List<TileMatrixLimits> GetLimits(string collectionId)
    {
        var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
        if (collectionOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The source collection with ID = {collectionId} does not exists");
        }

        var result = new List<TileMatrixLimits>();
        for (var i = 0; i <= 22; i++)
        {
            result.Add(new TileMatrixLimits
            {
                TileMatrix = i,
                MinTileCol = 0,
                MaxTileCol = (1 << i) - 1,
                MinTileRow = 0,
                MaxTileRow = (1 << i) - 1
            });
        }
        return result;
    }

    public Dictionary<string, string> GetPropertyMetadata(string collectionId)
    {
        var collectionOptions = (CollectionOptions)CollectionsOptions.GetSourceById(collectionId);
        if (collectionOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was not found in the provided options", collectionId);
            return null;
        }
        var sourceOptions = (SqlFeaturesSourceOptions)collectionOptions.Features?.Storage;
        if (sourceOptions == null)
        {
            Logger.LogTrace(
                "The source collection with ID = {collectionId} was found, yet it contains no storage options", collectionId);
            return null;
        }

        using var connection = GetDbConnection(sourceOptions.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT 
                COLUMN_NAME,
                DATA_TYPE
            FROM 
                INFORMATION_SCHEMA.COLUMNS
            WHERE 
                TABLE_NAME = @Table
                AND TABLE_SCHEMA = @Schema;
            """;

        var tableParameter = command.CreateParameter();
        tableParameter.ParameterName = "Table";
        tableParameter.Value = sourceOptions.Table;

        var schemaParameter = command.CreateParameter();
        schemaParameter.ParameterName = "Schema";
        schemaParameter.Value = sourceOptions.Schema;

        command.Parameters.Add(tableParameter);
        command.Parameters.Add(schemaParameter);

        using var reader = command.ExecuteReader();

        var result = new Dictionary<string, string>();

        while (reader.Read())
        {
            var name = reader.GetString(0);
            var type = reader.GetString(1);

            result.Add(name, DbTypeMapper.MapDbTypeToSimpleType(type));
        }

        return result;
    }
}