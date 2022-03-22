using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options;
using System;

namespace OgcApi.Net.Features.PostGis
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiPostGisProvider(
            this IServiceCollection services, Action<CollectionsOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddTransient<IDataProvider, PostGisProvider>();

            return services;
        }
    }
}