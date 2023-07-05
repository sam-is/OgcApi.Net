using System;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Resources;

public class Link
{
    public Uri Href { get; set; }

    public string Rel { get; set; }

    public string Type { get; set; }

    [JsonPropertyName("hreflang")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string HrefLang { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Length { get; set; }
}