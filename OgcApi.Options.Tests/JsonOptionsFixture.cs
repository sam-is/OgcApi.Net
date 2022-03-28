using OgcApi.Options.Tests.Utils;
using OgcApi.Net.Options;

namespace OgcApi.Options.Tests
{
    public class JsonOptionsFixture
    {
        public readonly OgcApiOptions Options;

        public JsonOptionsFixture()
        {
            OptionsUtils.SetupServiceCollection();
            Options = OptionsUtils.GetOptionsFromJson();
        }
    }
}