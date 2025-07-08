using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Features;
using OgcApi.Net.Options.Features;

namespace OgcApi.Net.Schemas.Tests.Mock;

[OgcFeaturesProvider("Test", typeof(SqlFeaturesSourceOptions))]
internal class MockFeaturesProvider : IFeaturesProvider, IPropertyMetadataProvider
{
    public virtual Dictionary<string, string> GetPropertyMetadata(string collectionId) => new()
    {
        ["Id"] = "number",
        ["Number"] = "number",
        ["String"] = "string",
        ["Geometry"] = "geometry",
        ["Date"] = "string"
    };

    public string CreateFeature(string collectionId, IFeature feature, string? apiKey = null)
    {
        throw new NotImplementedException();
    }

    public void DeleteFeature(string collectionId, string featureId, string? apiKey = null)
    {
        throw new NotImplementedException();
    }

    public Envelope GetBbox(string collectionId, string? apiKey = null)
    {
        throw new NotImplementedException();
    }

    public OgcFeature GetFeature(string collectionId, string featureId, string? apiKey = null)
    {
        throw new NotImplementedException();
    }

    public OgcFeatureCollection GetFeatures(string collectionId, int limit = 10, int offset = 0, Envelope? bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string? apiKey = null, Dictionary<string, string>? propertyFilter = null)
    {
        throw new NotImplementedException();
    }

    public long GetFeaturesCount(string collectionId, Envelope? bbox = null, DateTime? startDateTime = null, DateTime? endDateTime = null, string? apiKey = null, Dictionary<string, string>? propertyFilter = null)
    {
        throw new NotImplementedException();
    }

    public void ReplaceFeature(string collectionId, string featureId, IFeature feature, string? apiKey = null)
    {
        throw new NotImplementedException();
    }

    public void UpdateFeature(string collectionId, string featureId, IFeature feature, string? apiKey = null)
    {
        throw new NotImplementedException();
    }
}
