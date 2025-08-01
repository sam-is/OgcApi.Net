using OgcApi.Net.Options.Converters;
using OgcApi.Net.Options.Tests.Utils;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace OgcApi.Net.Options.Tests;

public class OgcApiOptionsSerializationFacts
{
    [Fact]
    public void OgcApiOptionsFromCodeSerialization()
    {
        var ogcApiOptions = OptionsUtils.GetOptionsFromCode();
        var json = JsonSerializer.Serialize(ogcApiOptions, OptionsUtils.SerializerOptions);

        Assert.False(string.IsNullOrEmpty(json));
    }
    [Fact]
    public void OgcApiOptionsFromConfigSerialization()
    {
        var ogcApiOptions = OptionsUtils.GetOptionsFromJsonConfig();
        var json = JsonSerializer.Serialize(ogcApiOptions, OptionsUtils.SerializerOptions);

        Assert.False(string.IsNullOrEmpty(json));
    }

    [Fact]
    public void LandingPageOptionsSerialization()
    {
        var ogcApiOptions = OptionsUtils.GetOptionsFromCode();
        var json = JsonSerializer.Serialize(ogcApiOptions.LandingPage, OptionsUtils.SerializerOptions);

        Assert.False(string.IsNullOrEmpty(json));
    }

    [Fact]
    public void CollectionsOptionsSerialization()
    {
        var ogcApiOptions = OptionsUtils.GetOptionsFromCode();
        var json = JsonSerializer.Serialize(ogcApiOptions.Collections, OptionsUtils.SerializerOptions);

        Assert.False(string.IsNullOrEmpty(json));
    }
    [Fact]
    public void OgcApiOptionsFromConfigWithDelegatesSerialization()
    {
        var ogcApiOptions = OptionsUtils.GetOptionsFromJsonConfig();

        foreach (var item in ogcApiOptions.Collections.Items.Where(x => x.Tiles != null))
            item.Tiles.Storage.TileAccessDelegate = static (collectionId, tileMatrix, tileRow, tileCol, apiKey) => true;

        var json = JsonSerializer.Serialize(ogcApiOptions, OptionsUtils.SerializerOptions);

        Assert.False(string.IsNullOrEmpty(json));
    }
}