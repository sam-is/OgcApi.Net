using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;

namespace OgcApi.Net.MbTiles;

public static class MbTilesProviderServiceCollectionExtensions
{
    public static IServiceCollection AddOgcApiMbTilesProvider(this IServiceCollection services)
    {
        services.AddSingleton<ITilesProvider, MbTilesProvider>();

        return services;
    }
}