using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.SqlServer.Options;
using System;

namespace OgcApi.Net.Features.SqlServer
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiSqlServerProvider(
            this IServiceCollection services, Action<SqlServerCollectionSourcesOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddTransient<IDataProvider, SqlServerProvider>();

            return services;
        }
    }
}