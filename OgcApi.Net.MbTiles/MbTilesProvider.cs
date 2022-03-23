using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OgcApi.Net.MbTiles
{
    public class MbTilesProvider : TilesDataProvider 
    {
        public override string SourceType => "Spatialite";
   
        public MbTilesProvider(IOptionsMonitor<TileSourcesOptions> tileSourcesOptions, ILogger logger)
    : base(tileSourcesOptions, logger)
        {

        }

        private SqliteConnection GetDbConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }

        private SqliteCommand GetDbCommand(string commandText, SqliteConnection dbConnection)
        {
            var command = dbConnection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }

        public override List<TileMatrixLimits> GetLimits(string collectionId)
        {
            var tileOptions = (MbTilesSourceOptions)TilesOptions.GetSourceById(collectionId);
            if (tileOptions == null)
            {
                Logger.LogTrace(
                    $"The tile source with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The tile source with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(tileOptions.ConnectionString);
                connection.Open();

                var command = GetDbCommand(@"SELECT zoom_level, MIN(tile_column), MAX(tile_column), MIN((1 << zoom_level) - 1 - tile_row), MAX((1 << zoom_level) - 1 - tile_row) FROM tiles GROUP BY zoom_level ORDER BY zoom_level", connection);

                List <TileMatrixLimits> result = new();
                using (var reader = command.ExecuteReader())

                {
                    while (reader.Read())
                    {
                        result.Add(new TileMatrixLimits { TileMatrix = reader.GetInt32(0), MinTileCol = reader.GetInt32(1), MaxTileCol = reader.GetInt32(2), MinTileRow = reader.GetInt32(3), MaxTileRow = reader.GetInt32(4) });
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GetLimits database query completed with an exception");
                throw;
            }
        }

        public override async Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileCol, int tileRow, string apiKey = null)
        {
            var tileOptions = (MbTilesSourceOptions)TilesOptions.GetSourceById(collectionId);
            if (tileOptions == null)
            {
                Logger.LogTrace(
                    $"The tile source with ID = {collectionId} was not found in the provided options");
                throw new ArgumentException($"The tile source with ID = {collectionId} does not exists");
            }

            try
            {
                using var connection = GetDbConnection(tileOptions.ConnectionString);
                connection.Open();

                var command = GetDbCommand(@"SELECT tile_data FROM tiles WHERE zoom_level = $zoom_level AND tile_column = $tile_column AND tile_row = $tile_row", connection);

                command.Parameters.AddWithValue("$zoom_level", tileMatrix);
                command.Parameters.AddWithValue("$tile_column", tileCol);
                command.Parameters.AddWithValue("$tile_row", (1 << tileMatrix) - 1 - tileRow);
                return (byte[])(await command.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GetTileAsync database query completed with an exception");
                throw;
            }
        }

    }
}
