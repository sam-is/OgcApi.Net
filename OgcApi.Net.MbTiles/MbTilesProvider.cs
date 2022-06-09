using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Interfaces;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace OgcApi.Net.MbTiles
{
    [OgcTilesProvider("MbTiles", typeof(MbTilesSourceOptions))]

    public class MbTilesProvider : ITilesProvider
    {
        private readonly ILogger<MbTilesProvider> _logger;

        private readonly ICollectionsOptions _collectionsOptions;

        public MbTilesProvider(ILogger<MbTilesProvider> logger, IOptionsMonitor<OgcApiOptions> options)
        {
            _logger = logger;
            _collectionsOptions = options.CurrentValue.Collections;
        }

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
                _logger.LogTrace(
                    $"The tile source for collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The tile source for collection with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(tileOptions.FileName);
                connection.Open();

                var checkMetadataCommand = GetDbCommand(@"SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'metadata'", connection);
                if (checkMetadataCommand.ExecuteScalar() == null) return null;

                var getParamCommand = GetDbCommand(@"SELECT Value FROM metadata WHERE name = $name", connection);

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

                List<TileMatrixLimits> result = new();
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
                _logger.LogError(ex, "GetLimits database query completed with an exception");
                throw;
            }
        }

        public async Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey = null)
        {
            var tileOptions = (MbTilesSourceOptions)_collectionsOptions.GetSourceById(collectionId)?.Tiles?.Storage;
            if (tileOptions == null)
            {
                _logger.LogTrace(
                    $"The tile source for collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The tile source for collection with ID = {collectionId} does not exists");
            }

            if (!tileOptions.TileAccessDelegate?.Invoke(collectionId, tileMatrix, tileRow, tileCol, apiKey) ?? false)
            {
                _logger.LogTrace(
                    $"Unauthorized tile request: apiKey = {apiKey},  collectionId = {collectionId}, tileMatrix = {tileMatrix}, tileRow = {tileRow}, tileCol = {tileCol}");
                throw new TileAccessException("Unauthorized tile request");
            }


            if (tileOptions.MinZoom.HasValue && tileMatrix < tileOptions.MinZoom.Value)
                return null;

            if (tileOptions.MaxZoom.HasValue && tileMatrix > tileOptions.MaxZoom.Value)
                return null;

            try
            {
                return await GetTileDirectAsync(tileOptions.FileName, tileMatrix, tileRow, tileCol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTileAsync database query completed with an exception");
                throw;
            }
        }

        public static async Task<byte[]> GetTileDirectAsync(string fileName, int tileMatrix, int tileRow, int tileCol)
        {
            await using var connection = GetDbConnection(fileName);
            connection.Open();

            var command = GetDbCommand(@"SELECT tile_data FROM tiles WHERE zoom_level = $zoom_level AND tile_column = $tile_column AND tile_row = $tile_row", connection);

            command.Parameters.AddWithValue("$zoom_level", tileMatrix);
            command.Parameters.AddWithValue("$tile_column", tileCol);
            command.Parameters.AddWithValue("$tile_row", (1 << tileMatrix) - 1 - tileRow);
            return (byte[])await command.ExecuteScalarAsync();
        }
    }
}
