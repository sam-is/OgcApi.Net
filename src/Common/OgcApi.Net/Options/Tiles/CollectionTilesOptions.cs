using System;

namespace OgcApi.Net.Options.Tiles;

public class CollectionTilesOptions
{
    public Uri Crs { get; set; }

    public Uri TileMatrixSet { get; set; } = new("http://www.opengis.net/def/tilematrixset/OGC/1.0/WebMercatorQuad");

    public ITilesSourceOptions Storage { get; set; }
}