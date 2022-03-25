using OgcApi.Options.Tests.Utils;
using OgcApi.Net.Options;

namespace OgcApi.Options.Tests
{

    public class ConfigurationOptionsFixture
    {
        public readonly OgcApiOptions options;
        public ConfigurationOptionsFixture()
        {
            OptionsUtils.SetupServiceCollection();
            options = OptionsUtils.GetOptionsFromConfiguration();
        }
    }
}
