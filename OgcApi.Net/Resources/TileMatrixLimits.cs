namespace OgcApi.Net.Resources;

public class TileMatrixLimits
{
    public int TileMatrix { get; set; }
    public int MinTileRow { get; set; }
    public int MaxTileRow { get; set; }
    public int MinTileCol { get; set; }
    public int MaxTileCol { get; set; }
}