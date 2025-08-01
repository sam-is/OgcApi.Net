using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var attribute = type.GetCustomAttribute<OgcFeaturesProviderAttribute>();

            if (attribute != null)
                _providersOptionsTypes[attribute.Name] = attribute.OptionsType;
        }
    }

    public override IFeaturesSourceOptions Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        var storageType = jsonDocument.RootElement.GetProperty("Type").GetString()
            ?? throw new JsonException("Type element is not defined");

        return jsonDocument.RootElement.Deserialize(GetOptionType(storageType), options) as IFeaturesSourceOptions;
    }

    public override void Write(Utf8JsonWriter writer, IFeaturesSourceOptions value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }

    private Type GetOptionType(string typeName)
    {
        if (!_providersOptionsTypes.TryGetValue(typeName, out var optionsType))
            throw new JsonException($"Cannot find type with `{typeName}` {nameof(OgcFeaturesProviderAttribute)} value");

        if (optionsType == null)
            throw new JsonException($"Attribute {nameof(OgcFeaturesProviderAttribute)} with name `{typeName}` does not supply OptionsType");

        return optionsType;
    }
}