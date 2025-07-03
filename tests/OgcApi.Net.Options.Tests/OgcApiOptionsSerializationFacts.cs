using OgcApi.Net.Options.Converters;
using OgcApi.Options.Tests.Utils;
using System.Text.Json;
using Xunit;

namespace OgcApi.Options.Tests;

public class OgcApiOptionsSerializationFacts
{
    [Fact]
    public void OgcApiOptionsSerialization()
    {
        var ogcApiOptions = OptionsUtils.GetOptionsFromCode();
        var json = JsonSerializer.Serialize(ogcApiOptions, new JsonSerializerOptions
        {
            Converters = { new FeaturesSourceOptionsConverter() }
        });

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
}