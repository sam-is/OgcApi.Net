using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Interfaces;
using OgcApi.Net.Options.Tiles;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OgcApi.Net.MbTiles
{
    public class MbTilesProvider : ITilesProvider
    {
        public string SourceType => "MbTiles";

        public ICollectionsOptions CollectionsOptions { get; set; }

        private readonly ILogger<MbTilesProvider> _logger;

        public MbTilesProvider(ILogger<MbTilesProvider> logger)
        {
            _logger = logger;
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
            var tileOptions = (MbTilesSourceOptions)CollectionsOptions.GetSourceById(collectionId)?.Tiles?.Storage;
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

                getParamCommand.Parameters["$name"].Value = "maxzoom";
                if (!int.TryParse(getParamCommand.ExecuteScalar()?.ToString(), out var maxZoom)) return null;

                getParamCommand.Parameters["$name"].Value = "bounds";
                var boundsStr = getParamCommand.ExecuteScalar()?.ToString();
                if (boundsStr == null) return null;
                string[] coordStrs = boundsStr.Split(',');
                if (coordStrs.Length != 4) return null;
                if (!double.TryParse(coordStrs[0], out double lon1)) return null;
                if (!double.TryParse(coordStrs[1], out double lat1)) return null;
                if (!double.TryParse(coordStrs[2], out double lon2)) return null;
                if (!double.TryParse(coordStrs[3], out double lat2)) return null;

                List<TileMatrixLimits> result = new();
                for (int i = minZoom; i <= maxZoom; i++)
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
            var tileOptions = (MbTilesSourceOptions)CollectionsOptions.GetSourceById(collectionId)?.Tiles?.Storage;
            if (tileOptions == null)
            {
                _logger.LogTrace(
                    $"The tile source for collection with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The tile source for collection with ID = {collectionId} does not exists");
            }

            try
            {
                await using var connection = GetDbConnection(tileOptions.FileName);
                connection.Open();

                var command = GetDbCommand(@"SELECT tile_data FROM tiles WHERE zoom_level = $zoom_level AND tile_column = $tile_column AND tile_row = $tile_row", connection);

                command.Parameters.AddWithValue("$zoom_level", tileMatrix);
                command.Parameters.AddWithValue("$tile_column", tileCol);
                command.Parameters.AddWithValue("$tile_row", (1 << tileMatrix) - 1 - tileRow);
                return (byte[])(await command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTileAsync database query completed with an exception");
                throw;
            }
        }

        public async Task<byte[]> GetTileDirectAsync(string fileName, int tileMatrix, int tileRow, int tileCol)
        {
            try
            {
                await using var connection = GetDbConnection(fileName);
                connection.Open();

                var command = GetDbCommand(@"SELECT tile_data FROM tiles WHERE zoom_level = $zoom_level AND tile_column = $tile_column AND tile_row = $tile_row", connection);

                command.Parameters.AddWithValue("$zoom_level", tileMatrix);
                command.Parameters.AddWithValue("$tile_column", tileCol);
                command.Parameters.AddWithValue("$tile_row", (1 << tileMatrix) - 1 - tileRow);
                return (byte[])(await command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetTileDirectAsync database query completed with an exception");
                throw;
            }
        }


        public ITilesSourceOptions DeserializeTilesSourceOptions(string json, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<MbTilesSourceOptions>(json, options);
        }

        public void SerializeTilesSourceOptions(Utf8JsonWriter writer, ITilesSourceOptions storage)
        {
            JsonSerializer.Serialize(writer, storage as MbTilesSourceOptions);
        }
    }
}
