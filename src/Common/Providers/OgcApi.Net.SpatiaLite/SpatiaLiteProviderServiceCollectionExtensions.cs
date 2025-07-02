using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;

namespace OgcApi.Net.SpatiaLite;

public static class OgcApiServiceCollectionExtensions
{
    public static IServiceCollection AddOgcApiSpatiaLiteProvider(this IServiceCollection services)
    {
        services.AddSingleton<IFeaturesProvider, SpatiaLiteProvider>();
        services.AddSingleton<ITilesProvider, SpatiaLiteProvider>();

        return services;
    }
}