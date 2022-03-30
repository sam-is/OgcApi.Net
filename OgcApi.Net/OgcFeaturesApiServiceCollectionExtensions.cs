using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OgcApi.Net.OpenApi;
using OgcApi.Net.Options;

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