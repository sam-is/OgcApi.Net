using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options
{
    public class OgcApiOptionsConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(OgcApiOptions);
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => new OgcApiOptionsConverter();
    }
}
