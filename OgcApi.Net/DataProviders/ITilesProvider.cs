using OgcApi.Net.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OgcApi.Net.DataProviders;

public interface ITilesProvider
{
    Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileRow, int tileCol, string datetime = null, string apiKey = null);

    List<TileMatrixLimits> GetLimits(string collectionId);
}