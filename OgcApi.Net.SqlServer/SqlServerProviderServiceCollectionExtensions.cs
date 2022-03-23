using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;

namespace OgcApi.Net.SqlServer
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiSqlServerProvider(this IServiceCollection services)
        {
            services.AddSingleton<IDataProvider, SqlServerProvider>();

            return services;
        }
    }
}