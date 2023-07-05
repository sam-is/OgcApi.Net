using OgcApi.Net.Options.Features;
using OgcApi.Net.Options.Interfaces;
using OgcApi.Net.Options.Tiles;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Options;

public class CollectionOptions : ICollectionOptions
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Link> Links { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Extent Extent { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ItemType { get; set; }

    public Func<string, Uri> FeatureHtmlPage { get; set; }

    public CollectionFeaturesOptions Features { get; set; }

    public CollectionTilesOptions Tiles { get; set; }
}