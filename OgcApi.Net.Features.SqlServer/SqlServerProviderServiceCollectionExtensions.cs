using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using System;
using OgcApi.Net.Features.Options;

namespace OgcApi.Net.Features.SqlServer
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiSqlServerProvider(
            this IServiceCollection services, Action<SqlCollectionSourcesOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddTransient<IDataProvider, SqlServerProvider>();

            return services;
        }
    }
}