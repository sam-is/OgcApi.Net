using System.Text.Json.Serialization;

namespace OgcApi.Net.Schemas.Schema.Model;
public class OgcJsonSchemaProperty
{
    [JsonPropertyName("x-ogc-role")]
    public string? XOgcRole { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("format")]
    public string? Format { get; set; }

    [JsonPropertyName("x-ogc-propertySeq")]
    public int? XOgcPropertySeq { get; set; }
}
