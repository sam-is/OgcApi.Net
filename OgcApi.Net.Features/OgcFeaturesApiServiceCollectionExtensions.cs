using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.Options;
using System;

namespace OgcApi.Net.Features
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApi(
           this IServiceCollection services, Action<OgcApiOptions> configOptions)
        {            
            return services.Configure(configOptions);
        }
    }
}