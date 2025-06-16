using OgcApi.Net.Options.Features;
using OgcApi.Net.Schemas.Options;
using OgcApi.Net.Schemas.Schema;
using OgcApi.Net.Schemas.Schema.Model;

namespace OgcApi.Schemas.Tests;

public class SchemasFacts
{
    private const string BaseUrl = "https://example.com/ogc/api";

    [Fact]
    public void AllDataInOptionsTest()
    {
        var expected = new OgcJsonSchema
        {
            Id = new Uri($"{BaseUrl}/collections/test/schema"),
            Properties = new Dictionary<string, OgcJsonSchemaProperty>
            {
                {
                    "Id",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number",
                        XOgcRole = "id",
                        Title = "Id"
                    }
                },
                {
                    "Number",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number",
                        Title = "number"
                    }
                },
                {
                    "String",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string",
                        Title = "string"
                    }
                },
                {
                    "Date",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string",
                        Format = "date-time",
                        Title = "date"
                    }
                },
                {
                    "Geometry",
                    new OgcJsonSchemaProperty
                    {
                        Format = "geometry-polygon",
                        XOgcRole = "primary-geometry",
                        Title = "geometry"
                    }
                }
            }
        };

        var options = new SchemaCollectionOptions
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

        var schemaGenerator = new SchemaGenerator(null, null);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }

    [Fact]
    public void WitoutOptionsTest()
    {
        var expected = new OgcJsonSchema
        {
            Id = new Uri($"{BaseUrl}/collections/test/schema"),
            Properties = new Dictionary<string, OgcJsonSchemaProperty>
            {
                {
                    "Id",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number",
                        XOgcRole = "id"
                    }
                },
                {
                    "Number",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number"
                    }
                },
                {
                    "String",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string"
                    }
                },
                {
                    "Date",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string",
                        Format = "date-time"
                    }
                },
                {
                    "Geometry",
                    new OgcJsonSchemaProperty
                    {
                        Format = "geometry-polygon",
                        XOgcRole = "primary-geometry"
                    }
                }
            }
        };

        var options = new SchemaCollectionOptions
        {
            Id = "test",
            Features = new CollectionFeaturesOptions
            {
                Storage = new SqlFeaturesSourceOptions
                {
                    IdentifierColumn = "Id",
                    GeometryColumn = "Geometry",
                    DateTimeColumn = "Date",
                    GeometryGeoJsonType = "Polygon"
                }
            }
        };

        var featureProvider = MoqUtils.GetIFeatureProviderWithIPropertyMetadataProvider();

        var schemaGenerator = new SchemaGenerator(featureProvider, null);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }

    [Fact]
    public void TitleOnlyOptionsTest()
    {
        var expected = new OgcJsonSchema
        {
            Id = new Uri($"{BaseUrl}/collections/test/schema"),
            Properties = new Dictionary<string, OgcJsonSchemaProperty>
            {
                {
                    "Id",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number",
                        XOgcRole = "id",
                        Title = "id"
                    }
                },
                {
                    "Number",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number",
                        Title = "number"
                    }
                },
                {
                    "String",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string",
                        Title = "string"
                    }
                },
                {
                    "Date",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string",
                        Format = "date-time",
                        Title = "date"
                    }
                },
                {
                    "Geometry",
                    new OgcJsonSchemaProperty
                    {
                        Format = "geometry-polygon",
                        XOgcRole = "primary-geometry",
                        Title = "geometry"
                    }
                }
            }
        };

        var options = new SchemaCollectionOptions
        {
            Id = "test",
            Features = new CollectionFeaturesOptions
            {
                Storage = new SqlFeaturesSourceOptions
                {
                    IdentifierColumn = "Id",
                    GeometryColumn = "Geometry",
                    DateTimeColumn = "Date",
                    GeometryGeoJsonType = "Polygon"
                }
            },
            SchemaOptions = new SchemaOptions
            {
                Properties = new Dictionary<string, PropertyDescription>
                {
                    {
                        "Id",
                        new PropertyDescription
                        {
                            Title = "id"
                        }
                    },
                    {
                        "Number",
                        new PropertyDescription
                        {
                            Title = "number"
                        }
                    },
                    {
                        "String",
                        new PropertyDescription
                        {
                            Title = "string"
                        }
                    },
                    {
                        "Date",
                        new PropertyDescription
                        {
                            Title = "date"
                        }
                    },
                    {
                        "Geometry",
                        new PropertyDescription
                        {
                            Title = "geometry"
                        }
                    }
                }
            }
        };

        var featureProvider = MoqUtils.GetIFeatureProviderWithIPropertyMetadataProvider();

        var schemaGenerator = new SchemaGenerator(featureProvider, null);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }

    [Fact]
    public void MbtilesWitoutOptionsTest()
    {
        var expected = new OgcJsonSchema
        {
            Id = new Uri($"{BaseUrl}/collections/test/schema"),
            Properties = new Dictionary<string, OgcJsonSchemaProperty>
            {
                {
                    "id",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number"
                    }
                },
                {
                    "number",
                    new OgcJsonSchemaProperty
                    {
                        Type = "number"
                    }
                },
                {
                    "string",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string"
                    }
                },
                {
                    "date",
                    new OgcJsonSchemaProperty
                    {
                        Type = "string"
                    }
                },
                {
                    "geometry",
                    new OgcJsonSchemaProperty
                    {
                        Format = "geometry-polygon",
                        XOgcRole = "primary-geometry"
                    }
                }
            }
        };

        var options = new SchemaCollectionOptions
        {
            Id = "test"
        };

        var tilesProvider = MoqUtils.GetITilesProviderWithIPropertyMetadataProvider();

        var schemaGenerator = new SchemaGenerator(null, tilesProvider);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }
}