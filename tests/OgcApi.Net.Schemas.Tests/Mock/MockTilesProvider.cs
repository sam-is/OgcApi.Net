using OgcApi.Net.DataProviders;
using OgcApi.Net.Resources;

namespace OgcApi.Net.Schemas.Tests.Mock;

[OgcTilesProvider("Test", null)]
internal class MockTilesProvider : ITilesProvider, IPropertyMetadataProvider
{
    public Dictionary<string, string> GetPropertyMetadata(string collectionId) => new()
    {
        ["id"] = "number",
        ["number"] = "number",
        ["string"] = "string",
        ["geometry"] = "Polygon",
        ["date"] = "string"
    };

    public List<TileMatrixLimits> GetLimits(string collectionId)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> GetTileAsync(string collectionId, int tileMatrix, int tileRow, int tileCol, string? datetime = null, string? apiKey = null)
    {
        throw new NotImplementedException();
    }
}
