using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Tests.Utils;

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
