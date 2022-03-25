using OgcApi.Net.Options.Tiles;
using System.Collections.Generic;

namespace OgcApi.Net.MbTiles
{
    public class MbTilesSourceOptions : ITilesSourceOptions
    {
        public string Type { get; set; }

        public string ConnectionString { set; get; }

        public List<string> Validate()
        {
            var failureMessages = new List<string>();

            if (!Type.Equals("MbTiles"))
                failureMessages.Add("Parameter Type requires \"MbTiles\" value for the \"MbTiles\" tile source option");

            if (string.IsNullOrWhiteSpace(ConnectionString))
                failureMessages.Add("Parameter ConnectionString is required for the \"MbTiles\" tile source option");

            return failureMessages;
        }
    }
}
