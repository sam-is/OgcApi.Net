using System.Collections.Generic;

namespace OgcApi.Net.Options.Tiles
{
    public delegate bool TileAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey);
    public interface ITilesSourceOptions
    {
        TileAccessDelegate TileAccessDelegate { get; set; }

        string Type { get; set; }

        int? MinZoom { get; set; }

        int? MaxZoom { get; set; }

        List<string> Validate();
    }
}
