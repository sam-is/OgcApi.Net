using Moq;
using OgcApi.Net.DataProviders;

namespace OgcApi.Schemas.Tests;
internal static class MoqUtils
{
    public static IFeaturesProvider GetIFeatureProviderWithIPropertyMetadataProvider()
    {
        var mockProvider = new Mock<IFeaturesProvider>();
        mockProvider.As<IPropertyMetadataProvider>()
        .Setup(p => p.GetPropertyMetadata(It.IsAny<string>()))
        .Returns(new Dictionary<string, string>
        {
            ["Id"] = "number",
            ["Number"] = "number",
            ["String"] = "string",
            ["Geometry"] = "geometry",
            ["Date"] = "string"
        });

        return mockProvider.Object;
    }

    public static ITilesProvider GetITilesProviderWithIPropertyMetadataProvider()
    {
        var mockProvider = new Mock<ITilesProvider>();
        mockProvider.As<IPropertyMetadataProvider>()
        .Setup(p => p.GetPropertyMetadata(It.IsAny<string>()))
        .Returns(new Dictionary<string, string>
        {
            ["id"] = "number",
            ["number"] = "number",
            ["string"] = "string",
            ["geometry"] = "Polygon",
            ["date"] = "string"
        });

        return mockProvider.Object;
    }
}
