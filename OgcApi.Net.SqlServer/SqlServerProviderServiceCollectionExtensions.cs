using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;
using System;
using OgcApi.Net.Options.SqlOptions;

namespace OgcApi.Net.SqlServer
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiSqlServerProvider(
            this IServiceCollection services, Action<SqlCollectionSourcesOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddTransient<IFeaturesProvider, SqlServerProvider>();

            return services;
        }
    }
}