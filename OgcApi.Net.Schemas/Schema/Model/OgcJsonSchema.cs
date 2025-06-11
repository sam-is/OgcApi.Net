using System.Text.Json.Serialization;

namespace OgcApi.Net.Schemas.Schema.Model;
public class OgcJsonSchema
{
    [JsonPropertyName("$schema")]
    public Uri Schema { get; set; } = new Uri("https://json-schema.org/draft/2020-12/schema");

    [JsonPropertyName("$id")]
    public required Uri Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "object";

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("additionalProperties")]
    public bool? AdditionalProperties { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, OgcJsonSchemaProperty> Properties { get; set; } = [];
}
