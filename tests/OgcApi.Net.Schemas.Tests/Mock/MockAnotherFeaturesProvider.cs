using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Features;

namespace OgcApi.Net.Schemas.Tests.Mock;

[OgcFeaturesProvider("AnotherTest", typeof(SqlFeaturesSourceOptions))]
internal class MockAnotherFeaturesProvider : MockFeaturesProvider
{
    public override Dictionary<string, string> GetPropertyMetadata(string collectionId) => new()
    {
        ["another Id"] = "number",
        ["another Number"] = "number",
        ["another String"] = "string",
        ["another Geometry"] = "geometry",
        ["another Date"] = "string"
    };
}
