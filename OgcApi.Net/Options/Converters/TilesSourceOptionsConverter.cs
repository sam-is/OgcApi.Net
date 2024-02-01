using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Options.Converters;

public class TilesSourceOptionsConverter : JsonConverter<ITilesSourceOptions>
{
    private readonly Dictionary<string, Type> _providersOptionsTypes = new();

    public TilesSourceOptionsConverter()
    {
        var providersTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.FullName!.Contains("OgcApi"))
            .SelectMany(x => x.DefinedTypes)
            .Where(type => Attribute.IsDefined(type, typeof(OgcTilesProviderAttribute)));

        foreach (var type in providersTypes)
        {
            var attribute =
                (OgcTilesProviderAttribute)Attribute.GetCustomAttribute(type,
                    typeof(OgcTilesProviderAttribute));

            if (attribute != null)
                _providersOptionsTypes[attribute.Name] = attribute.OptionsType;
        }
    }

    public override ITilesSourceOptions Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        var storageType = jsonDocument.RootElement.GetProperty("Type").GetString() ?? throw new JsonException("Type element is not defined");
        var optionsType = _providersOptionsTypes[storageType];
        if (optionsType != null)
        {
            return JsonSerializer.Deserialize(jsonDocument.RootElement.ToString(), optionsType, options) as ITilesSourceOptions;
        }

        throw new JsonException($"Cannot find type with {storageType} OgcTilesProviderAttribute value");
    }

    public override void Write(Utf8JsonWriter writer, ITilesSourceOptions value, JsonSerializerOptions options)
    {
        var optionsType = _providersOptionsTypes[value.Type];
        if (optionsType != null)
        {
            JsonSerializer.Serialize(writer, value, optionsType, options);
        }
        else
        {
            throw new JsonException($"Cannot find type with {value.Type} OgcTilesProviderAttribute value");
        }
    }
}