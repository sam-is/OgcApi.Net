using System;

namespace OgcApi.Net.Options.TileOptions
{
    public class TileSourceOptions
    {
        public string Id { get; set; }

        public Uri TileMatrixSet { get; set; }

        public Uri Crs { get; set; }
    }
}
