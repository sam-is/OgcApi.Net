
using OgcApi.Net.Features.Options.Interfaces;
using OgcApi.Net.Options.Tiles;
using OgcApi.Net.Resources;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OgcApi.Net.DataProviders
{
    public interface ITilesProvider
    {
        string SourceType { get; }

        ICollectionsOptions CollectionsOptions { get; set; }

        Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey = null);

        List<TileMatrixLimits> GetLimits(string collectionId);

        ITilesSourceOptions DeserializeTilesSourceOptions(string json, JsonSerializerOptions options);
    }
}
