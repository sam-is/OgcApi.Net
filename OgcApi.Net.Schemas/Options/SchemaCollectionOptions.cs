using OgcApi.Net.Options;

namespace OgcApi.Net.Schemas.Options;

/// <summary>
/// Extends <see cref="CollectionOptions"/> adding OGC Schemas support.<br/>
/// <inheritdoc/>
/// </summary>
public class SchemaCollectionOptions : CollectionOptions
{
    /// <summary>
    /// Information about collection and its properties
    /// </summary>
    public SchemaOptions? SchemaOptions { get; set; }
}