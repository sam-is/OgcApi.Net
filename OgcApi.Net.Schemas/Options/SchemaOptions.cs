namespace OgcApi.Net.Schemas.Options;

public class SchemaOptions
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? AdditionalProperties { get; set; }
    public Dictionary<string, PropertyDescription> Properties { get; set; } = [];
}