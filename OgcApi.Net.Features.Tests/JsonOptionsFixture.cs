using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Tests.Utils;

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