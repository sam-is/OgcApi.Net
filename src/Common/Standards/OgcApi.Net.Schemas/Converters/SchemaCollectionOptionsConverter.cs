using OgcApi.Net.Options;
using OgcApi.Net.Schemas.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Schemas.Converters;

public class SchemaCollectionOptionsConverter : JsonConverter<CollectionOptions>
{
    public override bool CanConvert(Type typeToConvert) => typeof(CollectionOptions).IsAssignableFrom(typeToConvert);

    public override CollectionOptions? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        return jsonDocument.RootElement.Deserialize<SchemaCollectionOptions>(GetJsonOptionsWithoutSchemaConverter(options));
    }

    public override void Write(Utf8JsonWriter writer, CollectionOptions value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, typeof(SchemaCollectionOptions), GetJsonOptionsWithoutSchemaConverter(options));
    }

    private static JsonSerializerOptions GetJsonOptionsWithoutSchemaConverter(JsonSerializerOptions options)
    {
        var result = new JsonSerializerOptions(options);
        result.Converters.Remove(
            result.Converters.FirstOrDefault(c => c is SchemaCollectionOptionsConverter)!
        );

        return result;
    }
}