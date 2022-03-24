using OgcApi.Net.Features.Tests.Utils;
using OgcApi.Net.Options;

namespace OgcApi.Net.Features.Tests
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