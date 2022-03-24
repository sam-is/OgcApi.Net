using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OgcApi.Net.Options.Converters;
using System;
using System.IO;
using System.Text.Json;

namespace OgcApi.Net.Options
{
    public class ConfigureOgcApiOptions : IConfigureOptions<OgcApiOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ConfigureOgcApiOptions(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(OgcApiOptions options)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            var jsonOgcApiSettings = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ogcsettings.json"));
            var reader = new Utf8JsonReader(jsonOgcApiSettings);
            var serializerOptions = new JsonSerializerOptions();
            var collectionOptionsConverter = new OgcApiOptionsConverter(provider);

            options.LandingPage = collectionOptionsConverter.ReadLandingPage(ref reader);
            options.Conformance = collectionOptionsConverter.ReadConformance(ref reader, serializerOptions);
            options.UseApiKeyAuthorization = collectionOptionsConverter.ReadUseApiKeyAuthorization(ref reader);
            options.Collections = collectionOptionsConverter.ReadCollections(ref reader, serializerOptions);
        }
    }
}