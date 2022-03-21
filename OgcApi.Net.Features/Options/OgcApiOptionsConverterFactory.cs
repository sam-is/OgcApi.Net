using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace OgcApi.Net.Features.Options
{
    public class OgcApiOptionsConverterFactory : JsonConverterFactory
    {
        public OgcApiOptionsConverterFactory (IServiceProvider provider)
        {
            Provider = provider;
        }

        private readonly IServiceProvider Provider;

        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(OgcApiOptions);
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => new OgcApiOptionsConverter(Provider);
    }
}
