using OgcApi.Net.Options;
using OgcApi.Net.Schemas.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Schemas.Converters;

public class SchemaCollectionOptionsConverter : JsonConverter<CollectionOptions>
{
    public override bool CanConvert(Type typeToConvert) => typeof(CollectionOptions).IsAssignableFrom(typeToConvert);

    public override CollectionOptions Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonOptions = new JsonSerializerOptions(options)
        {
            WriteIndented = options.WriteIndented,
            PropertyNameCaseInsensitive = options.PropertyNameCaseInsensitive
        };

        jsonOptions.Converters.Clear();
        foreach (var converter in options.Converters.Where(c => c != this))
            jsonOptions.Converters.Add(converter);

        var root = JsonDocument.ParseValue(ref reader).RootElement;

        if (root.Deserialize<SchemaCollectionOptions>(jsonOptions) is CollectionOptions result)
        {
            reader.Skip();
            return result;
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, CollectionOptions value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, options);
}