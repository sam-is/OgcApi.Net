using OgcApi.Net.Options.Tiles;

namespace OgcApi.Net.Schemas.Tests.Mock;

public class MockTileSourceOptions : ITilesSourceOptions
{
    public string? Type { get; set; }

    public string? FileName { get; set; }

    public int? MinZoom { get; set; }

    public int? MaxZoom { get; set; }

    public TileAccessDelegate? TileAccessDelegate { get; set; }
    public FeatureAccessDelegate? FeatureAccessDelegate { get; set; }

    public List<string> Validate()
    {
        return [];
    }
}