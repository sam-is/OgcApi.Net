using OgcApi.Net.Resources;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Styles.Model.Styles;

/// <summary>
/// Represents a style entry exposing its identifier, optional title and links to available stylesheets.
/// </summary>
public class OgcStyle
{
    /// <summary>
    /// Style identifier
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    /// <summary>
    /// Style title
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Links to the stylesheets of the style
    /// </summary>
    [JsonPropertyName("links")]
    public List<Link> Links { get; set; } = [];
}