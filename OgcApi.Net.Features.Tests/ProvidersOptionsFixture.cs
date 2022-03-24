using OgcApi.Net.DataProviders;
using OgcApi.Net.Features.Tests.Utils;
using OgcApi.Net.Options;

namespace OgcApi.Net.Features.Tests
{ 

    public class ProvidersOptionsFixture
    {
        public readonly CollectionsOptions jsonCollectionsOptions;
        public readonly CollectionsOptions sqlServerCollectionsOptions;
        public readonly CollectionsOptions postGisCollectionsOptions;
        public readonly IFeaturesProvider sqlServerProvider;
        public readonly IFeaturesProvider postGisProvider;

        public ProvidersOptionsFixture()
        {
            OptionsUtils.SetupServiceCollection();
            OptionsUtils.SetupServiceCollection();
            jsonCollectionsOptions = OptionsUtils.GetOptionsFromJson().Collections;
            sqlServerProvider = OptionsUtils.GetDataProvider("SqlServer");
            postGisProvider = OptionsUtils.GetDataProvider("PostGis");
            sqlServerCollectionsOptions = sqlServerProvider.CollectionsOptions as CollectionsOptions;
            postGisCollectionsOptions = postGisProvider.CollectionsOptions as CollectionsOptions;
        }
    }
}
