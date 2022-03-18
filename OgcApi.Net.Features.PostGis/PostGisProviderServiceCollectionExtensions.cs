using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;

namespace OgcApi.Net.Features.PostGis
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApiPostGisProvider(
            this IServiceCollection services)
        {        
            services.AddSingleton<IDataProvider, PostGisProvider>();

            return services;
        }
    }
}