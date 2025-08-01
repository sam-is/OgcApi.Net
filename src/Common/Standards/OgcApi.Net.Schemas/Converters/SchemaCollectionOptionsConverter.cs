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

        // ? jsonOptions.Converters.Remove(this);
        jsonOptions.Converters.Clear();
        foreach (var converter in options.Converters.Where(c => c != this))
            jsonOptions.Converters.Add(converter);

        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        if (jsonDocument.RootElement.Deserialize<SchemaCollectionOptions>(jsonOptions) is { } result)
        {
            reader.Skip();
            return result;
        }

        throw new JsonException($"Cannot read json element `{jsonDocument.RootElement}` as {nameof(SchemaCollectionOptions)}.");
    }

    public override void Write(Utf8JsonWriter writer, CollectionOptions value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, value, options);
}