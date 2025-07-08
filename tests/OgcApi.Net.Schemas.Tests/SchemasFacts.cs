using OgcApi.Net.Options.Features;
using OgcApi.Net.Schemas.Options;
using OgcApi.Net.Schemas.Schema;
using OgcApi.Net.Schemas.Schema.Model;
using OgcApi.Net.Schemas.Tests.Mock;

namespace OgcApi.Net.Schemas.Tests;

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

        var serviceProvider = MockUtils.CreateEmptyServiceProvider();

        var schemaGenerator = new SchemaGenerator(serviceProvider);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }

    [Fact]
    public void WithoutOptionsTest()
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
                    GeometryGeoJsonType = "Polygon",
                    Type = "Test"
                }
            }
        };

        var serviceProvider = MockUtils.CreateServiceProviderWithFeaturesProvider();

        var schemaGenerator = new SchemaGenerator(serviceProvider);

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
                    GeometryGeoJsonType = "Polygon",
                    Type = "Test"
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

        var serviceProvider = MockUtils.CreateServiceProviderWithFeaturesProvider();

        var schemaGenerator = new SchemaGenerator(serviceProvider);

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

        var serviceProvider = MockUtils.CreateServiceProviderWithTilesProviders();

        var schemaGenerator = new SchemaGenerator(serviceProvider);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }

    [Fact]
    public void TitleOnlyOptionsWithAllPropertiesTest()
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
            Features = new CollectionFeaturesOptions
            {
                Storage = new SqlFeaturesSourceOptions
                {
                    IdentifierColumn = "Id",
                    GeometryColumn = "Geometry",
                    DateTimeColumn = "Date",
                    GeometryGeoJsonType = "Polygon",
                    Type = "Test",
                    Properties = ["Number", "String", "Date"]
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
                            Title = "Id"
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

        var serviceProvider = MockUtils.CreateServiceProviderWithFeaturesProvider();

        var schemaGenerator = new SchemaGenerator(serviceProvider);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }

    [Fact]
    public void TitleOnlyOptionsWithSomePropertiesTest()
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
                    GeometryGeoJsonType = "Polygon",
                    Type = "Test",
                    Properties = ["Number"]
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
                            Title = "Id"
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

        var serviceProvider = MockUtils.CreateServiceProviderWithFeaturesProvider();

        var schemaGenerator = new SchemaGenerator(serviceProvider);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }

    [Fact]
    public void ManyTest()
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
                    GeometryGeoJsonType = "Polygon",
                    Type = "Test"
                }
            }
        };

        var serviceProvider = MockUtils.CreateServiceProviderWithFeaturesProviders();

        var schemaGenerator = new SchemaGenerator(serviceProvider);

        var actual = schemaGenerator.GenerateSchema(new Uri($"{BaseUrl}/"), options);

        Assert.Equal(expected.Id, actual.Id);

        foreach (var property in actual.Properties)
            Assert.Equivalent(expected.Properties.FirstOrDefault(p => p.Key == property.Key), property);
    }
}