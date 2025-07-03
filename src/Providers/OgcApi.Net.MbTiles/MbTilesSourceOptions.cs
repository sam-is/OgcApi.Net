using OgcApi.Net.Options.Tiles;
using System.Collections.Generic;

namespace OgcApi.Net.MbTiles;

public class MbTilesSourceOptions : ITilesSourceOptions
{
    public string Type { get; set; }

    public string FileName { get; set; }

    public int? MinZoom { get; set; }

    public int? MaxZoom { get; set; }

    public TileAccessDelegate TileAccessDelegate { get; set; }
    public FeatureAccessDelegate FeatureAccessDelegate { get; set; }

    public List<TimestampFile> TimestampFiles { get; set; } = [];

    public List<string> Validate()
    {
        var failureMessages = new List<string>();

        if (!Type.Equals("MbTiles"))
            failureMessages.Add("Parameter Type requires \"MbTiles\" value for the \"MbTiles\" tile source option");

        if (string.IsNullOrWhiteSpace(FileName))
            failureMessages.Add("Parameter FileName is required for the \"MbTiles\" tile source option");

        return failureMessages;
    }
}