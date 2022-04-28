using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.OpenApi;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Converters;
using System;
using System.IO;
using System.Text.Json;

namespace OgcApi.Net
{
    public static class OgcApiServiceCollectionExtensions
    {
        public static IServiceCollection AddOgcApi(this IServiceCollection services, string settingsFileName)
        {
            services.Configure<OgcApiOptions>(options =>
            {
                var ogcApiOptions = JsonSerializer.Deserialize<OgcApiOptions>(File.ReadAllBytes(settingsFileName),
                    new JsonSerializerOptions
                    {
                        Converters = { new FeaturesSourceOptionsConverter(), new TilesSourceOptionsConverter() }
                    });

                if (ogcApiOptions != null)
                {
                    options.Collections = ogcApiOptions.Collections;
                    options.Conformance = ogcApiOptions.Conformance;
                    options.LandingPage = ogcApiOptions.LandingPage;
                    options.UseApiKeyAuthorization = ogcApiOptions.UseApiKeyAuthorization;
                }
            });

            return services.AddSingleton<IOpenApiGenerator, OpenApiGenerator>();
        }

        public static IServiceCollection AddOgcApi(this IServiceCollection services, Action<OgcApiOptions> ogcApiOptions)
        {
            services.Configure(ogcApiOptions);
            return services.AddSingleton<IOpenApiGenerator, OpenApiGenerator>();
        }
    }
}