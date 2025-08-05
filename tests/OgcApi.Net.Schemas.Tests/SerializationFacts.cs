using OgcApi.Net.Options.Converters;
using OgcApi.Net.Schemas.Options;
using System.Text.Json;
using OgcApi.Net.Options;
using OgcApi.Net.Schemas.Converters;

namespace OgcApi.Net.Schemas.Tests;

public class SerializationFacts
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters =
        {
            new FeaturesSourceOptionsConverter(),
            new TilesSourceOptionsConverter(),
            new SchemaCollectionOptionsConverter()
        }
    };

    [Fact]
    public void Deserialize()
    {
        var collectionOptions =
            JsonSerializer.Deserialize<CollectionOptions>(
                File.ReadAllBytes(Path.Combine("Data", "SettingsWithSchema.json")), SerializerOptions);
        Assert.NotNull(collectionOptions);
        Assert.IsType<SchemaCollectionOptions>(collectionOptions);
    }

    [Fact]
    public void Serialize()
    {
        var schemasOptions = new SchemaCollectionOptions
        {
            Id = "test",
            SchemaOptions = new SchemaOptions
            {
                Properties = new Dictionary<string, PropertyDescription>
                {
                    {
                        "Id",
                        new PropertyDescription
                        {
                            Type = "number",
                            XOgcRole = "id",
                            Title = "Id"
                        }
                    },
                    {
                        "Number",
                        new PropertyDescription
                        {
                            Type = "number",
                            Title = "number"
                        }
                    },
                    {
                        "String",
                        new PropertyDescription
                        {
                            Type = "string",
                            Title = "string"
                        }
                    },
                    {
                        "Date",
                        new PropertyDescription
                        {
                            Type = "string",
                            Format = "date-time",
                            Title = "date"
                        }
                    },
                    {
                        "Geometry",
                        new PropertyDescription
                        {
                            Format = "geometry-polygon",
                            XOgcRole = "primary-geometry",
                            Title = "geometry"
                        }
                    }
                }
            }
        };

        var ogcApiOptions = new OgcApiOptions
        {
            Collections = new CollectionsOptions
            {
                Items = [schemasOptions]
            }
        };

        var serializedString = JsonSerializer.Serialize(ogcApiOptions, SerializerOptions);
        Assert.Contains("\"XOgcRole\":\"id\"", serializedString);
    }
}
