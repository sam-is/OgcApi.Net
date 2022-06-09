using OgcApi.Net.Options.Tiles;
using System.Collections.Generic;

namespace OgcApi.Net.MbTiles
{
    public class MbTilesSourceOptions : ITilesSourceOptions
    {
        public string Type { get; set; }

        public string FileName { set; get; }

        public int? MinZoom { get; set; }

        public int? MaxZoom { get; set; }

        TileAccessDelegate _TileAccessDelegate = null;
        public TileAccessDelegate TileAccessDelegate { get => _TileAccessDelegate; set => _TileAccessDelegate = value; }

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
}
