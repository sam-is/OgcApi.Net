using OgcApi.Net.Options;
using OgcApi.Options.Tests.Utils;

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