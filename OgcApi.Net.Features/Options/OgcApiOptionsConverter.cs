using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options
{
    public class OgcApiOptionsConverter : JsonConverter<OgcApiOptions>
    {
        public override OgcApiOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, OgcApiOptions value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
