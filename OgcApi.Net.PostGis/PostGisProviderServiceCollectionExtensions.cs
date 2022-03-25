using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;
using OgcApi.Net.PostGis;

namespace OgcApi.Net.Features.PostGis
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiPostGisProvider(this IServiceCollection services)
        {
            services.AddSingleton<IFeaturesProvider, PostGisProvider>();

            return services;
        }
    }
}