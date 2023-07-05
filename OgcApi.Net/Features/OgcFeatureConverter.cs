using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features;

public class OgcFeatureConverter : JsonConverter<OgcFeature>
{
    public override OgcFeature Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, OgcFeature value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        if (value.Links != null)
        {
            writer.WriteStartArray("links");
            foreach (var link in value.Links)
            {
                JsonSerializer.Serialize(writer, link, options);
            }
            writer.WriteEndArray();
        }

        writer.WriteString("type", "Feature");
        if (value.Id != null || options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
        {
            writer.WritePropertyName("id");
            JsonSerializer.Serialize(writer, value.Id, options);
        }
        if (value.Geometry != null || options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
        {
            writer.WritePropertyName("geometry");
            JsonSerializer.Serialize(writer, value.Geometry, options);
        }
        if (value.Attributes != null || options.DefaultIgnoreCondition != JsonIgnoreCondition.WhenWritingNull)
        {
            writer.WritePropertyName("properties");
            JsonSerializer.Serialize(writer, value.Attributes, options);
        }

        writer.WriteEndObject();
    }
}