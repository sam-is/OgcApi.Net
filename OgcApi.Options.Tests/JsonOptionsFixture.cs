using OgcApi.Options.Tests.Utils;
using OgcApi.Net.Options;

namespace OgcApi.Options.Tests
{

    public class JsonOptionsFixture
    {
        public readonly OgcApiOptions options;
        public JsonOptionsFixture()
        {
            OptionsUtils.SetupServiceCollection();
            options = OptionsUtils.GetOptionsFromJson();
        }
    }
}