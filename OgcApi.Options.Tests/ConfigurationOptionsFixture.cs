using OgcApi.Net.Options;
using OgcApi.Options.Tests.Utils;

namespace OgcApi.Options.Tests;

public class ConfigurationOptionsFixture
{
    public readonly OgcApiOptions Options = OptionsUtils.GetOptionsFromJsonConfig();
}