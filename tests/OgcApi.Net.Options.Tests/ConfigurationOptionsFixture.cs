using OgcApi.Net.Options.Tests.Utils;

namespace OgcApi.Net.Options.Tests;

public class ConfigurationOptionsFixture
{
    public readonly OgcApiOptions Options = OptionsUtils.GetOptionsFromJsonConfig();
}