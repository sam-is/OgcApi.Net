using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;

namespace OgcApi.Net.PostGis;

public static class OgcApiServiceCollectionExtensions
{
    public static IServiceCollection AddOgcApiPostGisProvider(this IServiceCollection services)
    {
        services.AddSingleton<IFeaturesProvider, PostGisProvider>();
        services.AddSingleton<ITilesProvider, PostGisProvider>();

        return services;
    }
}