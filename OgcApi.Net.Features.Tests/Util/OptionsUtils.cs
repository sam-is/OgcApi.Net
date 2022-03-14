using OgcApi.Net.Features.Options;
using Microsoft.Extensions.Configuration;

namespace OgcApi.Net.Features.Tests.Util
{
    public static class OptionsUtils
    {
        public static OgcApiOptions GetOptionsFromJson()
        {
            var config = new ConfigurationBuilder().AddJsonFile("//Util//appsettings_test.json").Build();
            var options = new OgcApiOptions();
            config.GetSection("FeaturesOptions").Bind(options);
            return options;
        }
    }
}
