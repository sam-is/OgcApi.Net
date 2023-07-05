using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;

namespace OgcApi.Net.SqlServer;

public static class OgcApiServiceCollectionExtensions
{
    public static IServiceCollection AddOgcApiSqlServerProvider(this IServiceCollection services)
    {
        services.AddSingleton<IFeaturesProvider, SqlServerProvider>();
        services.AddSingleton<ITilesProvider, SqlServerProvider>();

        return services;
    }
}