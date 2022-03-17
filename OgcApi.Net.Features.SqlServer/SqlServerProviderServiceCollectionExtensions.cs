using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options;
using System;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Net.Features.SqlServer
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiSqlServerProvider(
            this IServiceCollection services, Action<CollectionsOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddTransient<IDataProvider, SqlServerProvider>();

            return services;
        }
    }
}