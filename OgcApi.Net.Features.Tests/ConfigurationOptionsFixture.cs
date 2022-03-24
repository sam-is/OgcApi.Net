using OgcApi.Net.Features.Tests.Utils;
using OgcApi.Net.Options;

namespace OgcApi.Net.Features.Tests
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
