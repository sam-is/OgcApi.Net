using System.Text.Json.Serialization;

namespace OgcApi.Net.Styles.Model.Styles;

/// <summary>
/// Represents the styles available for a base resource (collection), including the default style id.
/// </summary>
public class OgcStyles
{
    /// <summary>
    /// Default style identifier
    /// </summary>
    [JsonPropertyName("default")]
    public string? Default { get; set; }

    /// <summary>
    /// Styles list
    /// </summary>
    [JsonPropertyName("styles")]
    public List<OgcStyle> Styles { get; set; } = [];
}