using System.Collections.Generic;

namespace OgcApi.Net.Options.Tiles
{
    public interface ITilesSourceOptions
    {
        string Type { get; set; }

        int? MinZoom { get; set; }

        int? MaxZoom { get; set; }

        List<string> Validate();
    }
}
