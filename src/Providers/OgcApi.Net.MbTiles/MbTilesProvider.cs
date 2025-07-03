using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OgcApi.Net.Crs;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Interfaces;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OgcApi.Net.MbTiles;

[OgcTilesProvider("MbTiles", typeof(MbTilesSourceOptions))]
public class MbTilesProvider(ILogger<MbTilesProvider> logger, IOptionsMonitor<OgcApiOptions> options)
    : ITilesProvider, IPropertyMetadataProvider
{
    private readonly ICollectionsOptions _collectionsOptions = options.CurrentValue.Collections;

    private static SqliteConnection GetDbConnection(string fileName)
    {
        return new SqliteConnection($"Data Source={fileName}");
    }

    private static SqliteCommand GetDbCommand(string commandText, SqliteConnection dbConnection)
    {
        var command = dbConnection.CreateCommand();
        command.CommandText = commandText;
        return command;
    }

    public List<TileMatrixLimits> GetLimits(string collectionId)
    {
        var tileOptions = (MbTilesSourceOptions)_collectionsOptions.GetSourceById(collectionId)?.Tiles?.Storage;
        if (tileOptions == null)
        {
            logger.LogTrace(
                "The tile source for collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The tile source for collection with ID = {collectionId} does not exists");
        }

        try
        {
            using var connection = GetDbConnection(tileOptions.FileName);
            connection.Open();

            var checkMetadataCommand = GetDbCommand("SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'metadata'", connection);
            if (checkMetadataCommand.ExecuteScalar() == null) return null;

            var getParamCommand = GetDbCommand("SELECT Value FROM metadata WHERE name = $name", connection);

            getParamCommand.Parameters.AddWithValue("$name", "minzoom");
            if (!int.TryParse(getParamCommand.ExecuteScalar()?.ToString(), out var minZoom)) return null;
            if (tileOptions.MinZoom.HasValue && minZoom < tileOptions.MinZoom.Value) minZoom = tileOptions.MinZoom.Value;

            getParamCommand.Parameters["$name"].Value = "maxzoom";
            if (!int.TryParse(getParamCommand.ExecuteScalar()?.ToString(), out var maxZoom)) return null;
            if (tileOptions.MaxZoom.HasValue && maxZoom > tileOptions.MaxZoom.Value) maxZoom = tileOptions.MaxZoom.Value;

            getParamCommand.Parameters["$name"].Value = "bounds";
            var boundsStr = getParamCommand.ExecuteScalar()?.ToString();
            if (boundsStr == null) return null;
            var coordStrs = boundsStr.Split(',');
            if (coordStrs.Length != 4) return null;
            if (!double.TryParse(coordStrs[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var lon1)) return null;
            if (!double.TryParse(coordStrs[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var lat1)) return null;
            if (!double.TryParse(coordStrs[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var lon2)) return null;
            if (!double.TryParse(coordStrs[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var lat2)) return null;

            List<TileMatrixLimits> result = [];
            for (var i = minZoom; i <= maxZoom; i++)
            {
                result.Add(new TileMatrixLimits
                {
                    TileMatrix = i,
                    MinTileCol = CoordinateConverter.LongToTileX(lon1, i),
                    MaxTileCol = CoordinateConverter.LongToTileX(lon2, i),
                    MinTileRow = CoordinateConverter.LatToTileY(lat2, i),
                    MaxTileRow = CoordinateConverter.LatToTileY(lat1, i)
                });
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetLimits database query completed with an exception");
            throw;
        }
    }

    public async Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileRow, int tileCol, string dateTime = null, string apiKey = null)
    {
        var tileOptions = (MbTilesSourceOptions)_collectionsOptions.GetSourceById(collectionId)?.Tiles?.Storage;
        if (tileOptions == null)
        {
            logger.LogTrace(
                "The tile source for collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The tile source for collection with ID = {collectionId} does not exists");
        }

        if (tileOptions.MinZoom.HasValue && tileMatrix < tileOptions.MinZoom.Value)
            return null;

        if (tileOptions.MaxZoom.HasValue && tileMatrix > tileOptions.MaxZoom.Value)
            return null;

        var fileName = tileOptions.FileName;

        if (dateTime != null)
        {
            var dateTimeInterval = Temporal.DateTimeInterval.Parse(dateTime);
            if (dateTimeInterval.Start != null)
            {
                fileName =
                    (from timestampFile in tileOptions.TimestampFiles
                     where dateTimeInterval.Start >= timestampFile.DateTime
                     orderby timestampFile.DateTime descending
                     select timestampFile.FileName).FirstOrDefault();

                if (fileName == null || !File.Exists(fileName))
                {
                    logger.LogError("GetTileAsync: file for collection with with datetime = {dateTime} ({fileName}) does not exist", dateTime, fileName);
                    return null;
                }
            }
        }

        try
        {
            return await GetTileDirectAsync(fileName, tileMatrix, tileRow, tileCol);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetTileAsync database query completed with an exception");
            throw;
        }
    }

    public static async Task<byte[]> GetTileDirectAsync(string fileName, int tileMatrix, int tileRow, int tileCol)
    {
        await using var connection = GetDbConnection(fileName);
        connection.Open();

        var command = GetDbCommand("SELECT tile_data FROM tiles WHERE zoom_level = $zoom_level AND tile_column = $tile_column AND tile_row = $tile_row", connection);

        command.Parameters.AddWithValue("$zoom_level", tileMatrix);
        command.Parameters.AddWithValue("$tile_column", tileCol);
        command.Parameters.AddWithValue("$tile_row", (1 << tileMatrix) - 1 - tileRow);
        return (byte[])await command.ExecuteScalarAsync();
    }

    public Dictionary<string, string> GetPropertyMetadata(string collectionId)
    {
        var tileOptions = (MbTilesSourceOptions)_collectionsOptions.GetSourceById(collectionId)?.Tiles?.Storage;
        if (tileOptions == null)
        {
            logger.LogTrace(
                "The tile source for collection with ID = {collectionId} was not found in the provided options", collectionId);
            throw new ArgumentException($"The tile source for collection with ID = {collectionId} does not exists");
        }

        using var connection = GetDbConnection(tileOptions.FileName);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            WITH LayerCount AS (
              SELECT COUNT(*) AS count
              FROM json_each((SELECT value FROM metadata WHERE name = 'json'), '$.tilestats.layers')
            ),
            TileLayerGeometry AS (
              SELECT 
                json_extract(tile_layer.value, '$.layer') AS layer_name,
                json_extract(tile_layer.value, '$.geometry') AS geometry_type
              FROM json_each((SELECT value FROM metadata WHERE name = 'json'), '$.tilestats.layers') AS tile_layer
            )
            SELECT 
              field.key AS field_name,
              field.value AS field_type
            FROM metadata m
            JOIN json_each(m.value, '$.vector_layers') AS layer
            JOIN json_each(json_extract(layer.value, '$.fields')) AS field
            CROSS JOIN LayerCount
            WHERE m.name = 'json'
              AND (json_extract(layer.value, '$.id') = @LayerName OR LayerCount.count <= 1)

            UNION

            SELECT 
              'geometry' AS field_name,
              geometry.geometry_type AS field_type
            FROM TileLayerGeometry geometry
            JOIN json_each(
              (SELECT value FROM metadata WHERE name = 'json'),
              '$.vector_layers'
            ) AS layer
            WHERE json_extract(layer.value, '$.id') = geometry.layer_name
              AND (geometry.layer_name = @LayerName OR (SELECT count FROM LayerCount) <= 1);
            """;

        command.Parameters.AddWithValue("LayerName", collectionId);

        using var reader = command.ExecuteReader();

        var result = new Dictionary<string, string>();

        while (reader.Read())
        {
            var name = reader.GetString(0);
            var type = reader.GetString(1);

            result.Add(name, CastTypeToSimple(type));
        }

        return result;
    }

    private static string CastTypeToSimple(string type) => type.ToLower() switch
    {
        "string" or "mixed" => "string",
        "number" => "number",
        "point" or "multipoint" or "linestring" or "multilinestring" or
        "polygon" or "multipolygon" or "geometrycollection" => type.ToLower(),
        _ => "unknown",
    };
}