using OgcApi.Net.Options.Features;
using OgcApi.Net.Options.Interfaces;
using OgcApi.Net.Options.Tiles;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Options;

/// <summary>
/// Information about the collection: id, title, features and tiles options, etc.
/// </summary>
/// <remarks>
/// Can be extended with inheritor  classes
/// </remarks>
public class CollectionOptions : ICollectionOptions
{
    /// <summary>
    /// Unique identifier of the collection
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// Title of the collection
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Description for the understanding of the collection. 
    /// </summary>
    public string Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<Link> Links { get; set; }

    /// <summary>
    /// Extent of all spatial objects in the collection.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Extent Extent { get; set; }

    /// <summary>
    /// An optional indicator about the type of the items in the collection (the default value, if the indicator is not provided, is 'feature')
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ItemType { get; set; }

    public Func<string, Uri> FeatureHtmlPage { get; set; }

    public CollectionFeaturesOptions Features { get; set; }

    public CollectionTilesOptions Tiles { get; set; }
}