using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Features;

public class OgcFeatureCollectionConverter : JsonConverter<OgcFeatureCollection>
{
    public override OgcFeatureCollection Read(ref Utf8JsonReader reader, Type objectType, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, OgcFeatureCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("type", "FeatureCollection");

        writer.WriteString("timeStamp", DateTime.Now);
        writer.WriteNumber("numberMatched", value.TotalMatched);
        writer.WriteNumber("numberReturned", value.Count);

        if (value.Links != null)
        {
            writer.WriteStartArray("links");
            foreach (var link in value.Links)
            {
                JsonSerializer.Serialize(writer, link, options);
            }
            writer.WriteEndArray();
        }

        writer.WriteStartArray("features");
        foreach (var feature in value)
            JsonSerializer.Serialize(writer, feature, options);
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}