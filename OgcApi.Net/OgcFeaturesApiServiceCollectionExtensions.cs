using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.OpenApi;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Converters;
using OgcApi.Net.Options.Features;
using OgcApi.Net.Options.Tiles;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net;

public static class OgcApiServiceCollectionExtensions
{
    public static IServiceCollection AddOgcApi(this IServiceCollection services, string settingsFileName, TileAccessDelegate tileAccessDelegate = null, FeatureAccessDelegate featureAccessDelegate = null)
    {
        services.Configure<OgcApiOptions>(options =>
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new FeaturesSourceOptionsConverter(), new TilesSourceOptionsConverter() }
            };

            var converters = services
                .BuildServiceProvider()
                .GetServices<JsonConverter<CollectionOptions>>();

            foreach (var converter in converters)
                jsonSerializerOptions.Converters.Add(converter);

            var ogcApiOptions = JsonSerializer.Deserialize<OgcApiOptions>(File.ReadAllBytes(settingsFileName), jsonSerializerOptions);

            if (ogcApiOptions == null) return;

            foreach (var item in ogcApiOptions.Collections.Items.Where(x => x.Tiles != null))
            {
                item.Tiles.Storage.TileAccessDelegate = tileAccessDelegate;
                item.Tiles.Storage.FeatureAccessDelegate = featureAccessDelegate;
            }

            options.Collections = ogcApiOptions.Collections;
            options.Conformance = ogcApiOptions.Conformance;
            options.LandingPage = ogcApiOptions.LandingPage;
            options.UseApiKeyAuthorization = ogcApiOptions.UseApiKeyAuthorization;
        });

        return services.AddSingleton<IOpenApiGenerator, OpenApiGenerator>();
    }

    public static IServiceCollection AddOgcApi(this IServiceCollection services, Action<OgcApiOptions> ogcApiOptions)
    {
        services.Configure(ogcApiOptions);
        return services.AddSingleton<IOpenApiGenerator, OpenApiGenerator>();
    }
}