namespace OgcApi.Net.Schemas.Options;

/// <summary>
/// Information about collection and its properties
/// </summary>
public class SchemaOptions
{
    /// <summary>
    /// Title of the collection. Usually same as <see cref="OgcApi.Net.Options.CollectionOptions.Title"/>
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Description of the collection. Usually same as <see cref="OgcApi.Net.Options.CollectionOptions.Description"/>
    /// </summary>
    public string? Description { get; set; }
    /// <summary>
    /// Indicates whether additional properties, not listed in <see cref="Properties"/>, are allowed in feature objects.
    /// </summary>
    /// <remarks>
    /// If set to <c>false</c>, properties that are not explicitly declared in the schema SHALL NOT be allowed, otherwise they SHALL be allowed. Default - <c>true</c>
    /// </remarks>
    public bool? AdditionalProperties { get; set; }
    /// <summary>
    /// Dictionary mapping property names to their <see cref="PropertyDescription">options</see>
    /// </summary>
    public Dictionary<string, PropertyDescription> Properties { get; set; } = [];
}