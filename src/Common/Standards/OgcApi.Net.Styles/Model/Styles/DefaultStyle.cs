using System.Text.Json.Serialization;

namespace OgcApi.Net.Styles.Model.Styles;

/// <summary>
/// DTO used to get or update the default style identifier for a collection.
/// </summary>
public class DefaultStyle
{
    /// <summary>
    /// Default style identifier
    /// </summary>
    [JsonPropertyName("default")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Default { get; set; }
}