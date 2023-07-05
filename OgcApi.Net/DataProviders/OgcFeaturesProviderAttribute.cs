using System;

namespace OgcApi.Net.DataProviders;

[AttributeUsage(AttributeTargets.Class)]
public class OgcFeaturesProviderAttribute : Attribute
{
    public string Name;

    public Type OptionsType;

    public OgcFeaturesProviderAttribute(string name, Type optionsType)
    {
        Name = name;
        OptionsType = optionsType;
    }
}