using System.Collections.Generic;

namespace OgcApi.Net.Options.Tiles
{
    public interface ITilesSourceOptions
    {
        string Type { get; set; }

        List<string> Validate();
    }
}
