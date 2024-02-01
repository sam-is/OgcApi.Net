using System;

namespace OgcApi.Net.DataProviders;

[AttributeUsage(AttributeTargets.Class)]
public class OgcFeaturesProviderAttribute(string name, Type optionsType) : Attribute
{
    public readonly string Name = name;

    public readonly Type OptionsType = optionsType;
}