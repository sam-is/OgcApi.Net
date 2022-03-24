using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Tests.Utils;

namespace OgcApi.Net.Features.Tests
{ 

    public class ProvidersOptionsFixture
    {
        public readonly CollectionsOptions jsonCollectionsOptions;
        public readonly CollectionsOptions sqlServerCollectionsOptions;
        public readonly CollectionsOptions postGisCollectionsOptions;
        public readonly IDataProvider sqlServerProvider;
        public readonly IDataProvider postGisProvider;

        public ProvidersOptionsFixture()
        {
            OptionsUtils.SetupServiceCollection();
            OptionsUtils.SetupServiceCollection();
            jsonCollectionsOptions = OptionsUtils.GetOptionsFromJson().Collections;
            sqlServerProvider = OptionsUtils.GetDataProvider("SqlServer");
            postGisProvider = OptionsUtils.GetDataProvider("PostGis");
            sqlServerCollectionsOptions = sqlServerProvider.GetCollectionSourcesOptions() as CollectionsOptions;
            postGisCollectionsOptions = postGisProvider.GetCollectionSourcesOptions() as CollectionsOptions;
        }
    }
}
