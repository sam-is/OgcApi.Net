using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Resources;

/// <summary>
/// Provides information about and access to the collections
/// </summary>
public class Collections
{
    public List<Link> Links { get; set; }

    [JsonPropertyName("collections")]
    public List<Collection> Items { get; set; }
}