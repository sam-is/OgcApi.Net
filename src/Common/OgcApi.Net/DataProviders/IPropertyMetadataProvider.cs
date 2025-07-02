using System.Collections.Generic;

namespace OgcApi.Net.DataProviders;
public interface IPropertyMetadataProvider
{
    public Dictionary<string, string> GetPropertyMetadata(string collectionId);
}
