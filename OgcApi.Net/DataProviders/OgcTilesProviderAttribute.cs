using System;

namespace OgcApi.Net.DataProviders;

[AttributeUsage(AttributeTargets.Class)]
public class OgcTilesProviderAttribute : Attribute
{
    public string Name;

    public Type OptionsType;

    public OgcTilesProviderAttribute(string name, Type optionsType)
    {
        Name = name;
        OptionsType = optionsType;
    }
}