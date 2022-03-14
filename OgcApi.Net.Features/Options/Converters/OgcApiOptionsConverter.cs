using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features.Options.Converters
{
    public class OgcApiOptionsConverter : JsonConverter<OgcApiOptions>
    {
        public override OgcApiOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, OgcApiOptions value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.LandingPage != null)
                JsonSerializer.Serialize(writer, value.LandingPage, options);

            if (value.Conformance != null)
                JsonSerializer.Serialize(writer, value.Conformance, options);

            writer.WriteBoolean(nameof(value.UseApiKeyAuthorization), value.UseApiKeyAuthorization);

            if (value.Collections != null)
            {
                if (value.Collections.Links.Any())
                {
                    writer.WriteStartArray("Links");
                    foreach (var link in value.Collections.Links)
                        writer.WriteStringValue(link.Href.ToString());
                    writer.WriteEndArray();
                }
                if (value.Collections.Items.Any())
                {
                    writer.WriteStartArray("Items");
                    foreach (var item in value.Collections.Items)
                        JsonSerializer.Serialize(writer, item, options);
                    writer.WriteEndArray();
                }
            }
            writer.WriteEndObject();
        }
    }
}
