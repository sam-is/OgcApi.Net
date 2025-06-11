using OgcApi.Net.Options;

namespace OgcApi.Net.Schemas.Options;
public class SchemaCollectionOptions : CollectionOptions
{
    public SchemaOptions? SchemaOptions { get; set; }
}