using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.SqlOptions;
using OgcApi.Net.PostGis;
using System;

namespace OgcApi.Net.Features.PostGis
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiPostGisProvider(
            this IServiceCollection services, Action<SqlCollectionSourcesOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddTransient<IFeaturesProvider, PostGisProvider>();

            return services;
        }
    }
}