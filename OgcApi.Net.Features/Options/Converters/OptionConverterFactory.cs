using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OgcApi.Net.Features.Options.Converters
{
    public class OptionConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(OgcApiOptions) || typeToConvert == typeof(CollectionOptions);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(OgcApiOptions))
                return new OgcApiOptionsConverter();
            else
                return new CollectionOptionsConverter();

        }
    }
}
