using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace OgcApi.Net.Options.Converters;

public class TilesSourceOptionsConverter : JsonConverter<ITilesSourceOptions>
{
    private readonly Dictionary<string, Type> _providersOptionsTypes = [];

    public TilesSourceOptionsConverter()
    {
        var providersTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => assembly.FullName!.Contains("OgcApi"))
            .SelectMany(x => x.DefinedTypes)
            .Where(type => Attribute.IsDefined(type, typeof(OgcTilesProviderAttribute)));

        foreach (var type in providersTypes)
        {
            var attribute = type.GetCustomAttribute<OgcTilesProviderAttribute>();

            if (attribute != null)
                _providersOptionsTypes[attribute.Name] = attribute.OptionsType;
        }
    }

    public override ITilesSourceOptions Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);

        var storageType = jsonDocument.RootElement.GetProperty("Type").GetString()
            ?? throw new JsonException("Type element is not defined");

        return jsonDocument.RootElement.Deserialize(GetOptionType(storageType), options) as ITilesSourceOptions;
    }

    public static void IgnoreDelegateProperties(JsonTypeInfo typeInfo)
    {
        foreach (var prop in typeInfo.Properties)
        {
            if (prop.PropertyType.IsSubclassOf(typeof(Delegate)))
            {
                // Should skip delegate properties
                prop.ShouldSerialize = static (context, value) => false;
            }
        }
    }
    public override void Write(Utf8JsonWriter writer, ITilesSourceOptions value, JsonSerializerOptions options)
    {
        var specialOptions = new JsonSerializerOptions(options);
        if (specialOptions.TypeInfoResolver == null)
        {
            specialOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers = { IgnoreDelegateProperties }
            };
        }
        else
        {
            specialOptions.TypeInfoResolver = specialOptions.TypeInfoResolver.WithAddedModifier(IgnoreDelegateProperties);
        }
        JsonSerializer.Serialize(writer, value, value.GetType(), specialOptions);
    }

    private Type GetOptionType(string typeName)
    {
        if (!_providersOptionsTypes.TryGetValue(typeName, out var optionsType))
            throw new JsonException($"Cannot find type with `{typeName}` {nameof(OgcTilesProviderAttribute)} value");

        if (optionsType == null)
            throw new JsonException($"Attribute {nameof(OgcTilesProviderAttribute)} with name `{typeName}` does not supply OptionsType");

        return optionsType;
    }
}