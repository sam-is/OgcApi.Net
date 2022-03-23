using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options.Converters
{
    public class OgcApiOptionsConverterFactory : JsonConverterFactory
    {
        public OgcApiOptionsConverterFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        private readonly IServiceProvider _provider;

        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(OgcApiOptions);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => new OgcApiOptionsConverter(_provider);
    }
}
