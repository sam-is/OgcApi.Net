using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Options.Converters;

public class FeaturesSourceOptionsConverter : JsonConverter<IFeaturesSourceOptions>
{
    private readonly Dictionary<string, Type> _providersOptionsTypes = [];

    public FeaturesSourceOptionsConverter()
    {
        var providersTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.FullName!.Contains("OgcApi"))
            .SelectMany(x => x.DefinedTypes)
            .Where(type => Attribute.IsDefined(type, typeof(OgcFeaturesProviderAttribute)));

        foreach (var type in providersTypes)
        {
            var attribute =
                (OgcFeaturesProviderAttribute)Attribute.GetCustomAttribute(type,
                    typeof(OgcFeaturesProviderAttribute));

            if (attribute != null)
                _providersOptionsTypes[attribute.Name] = attribute.OptionsType;
        }
    }

    public override IFeaturesSourceOptions Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        var storageType = jsonDocument.RootElement.GetProperty("Type").GetString() ?? throw new JsonException("Type element is not defined");
        var optionsType = _providersOptionsTypes[storageType];
        if (optionsType != null)
        {
            return JsonSerializer.Deserialize(jsonDocument.RootElement.ToString(), optionsType, options) as IFeaturesSourceOptions;
        }

        throw new JsonException($"Cannot find type with {storageType} OgcFeaturesProviderAttribute value");
    }

    public override void Write(Utf8JsonWriter writer, IFeaturesSourceOptions value, JsonSerializerOptions options)
    {
        var optionsType = _providersOptionsTypes[value.Type];
        if (optionsType != null)
        {
            JsonSerializer.Serialize(writer, value, optionsType, options);
        }
        else
        {
            throw new JsonException($"Cannot find type with {value.Type} OgcFeaturesProviderAttribute value");
        }
    }
}