using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;

namespace OgcApi.Net.Features.Options
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
            var converter = new OgcApiOptionsConverter(provider);
            var jsonOptions = converter.Read(ref reader, typeof(OgcApiOptions), new JsonSerializerOptions());
            options.LandingPage = jsonOptions.LandingPage;
            options.UseApiKeyAuthorization = jsonOptions.UseApiKeyAuthorization;
            options.Conformance = jsonOptions.Conformance;
            options.Collections = jsonOptions.Collections;
        }
    }
}