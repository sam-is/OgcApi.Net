using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Resources;

public class TileSets
{
    [JsonPropertyName("tilesets")]
    public List<TileSet> Items { get; set; }
}