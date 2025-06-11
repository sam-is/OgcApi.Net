using OgcApi.Net.Options;
using OgcApi.Net.Schemas.Schema.Model;

namespace OgcApi.Net.Schemas.Schema;
public interface ISchemaGenerator
{
    public OgcJsonSchema GenerateSchema(Uri baseUri, CollectionOptions collectionOptions);

    public OgcJsonSchema GenerateSortablesSchema(Uri baseUri, CollectionOptions collectionOptions);

    public OgcJsonSchema GenerateQueryablesSchema(Uri baseUri, CollectionOptions collectionOptions);
}
