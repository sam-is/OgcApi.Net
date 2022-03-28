using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Options;
using Microsoft.Extensions.Options;
using OgcApi.Net.OpenApi;

namespace OgcApi.Net
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApi(this IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<OgcApiOptions>, ConfigureOgcApiOptions>();
            return services.AddSingleton<IOpenApiGenerator, OpenApiGenerator>();
        }
    }
}