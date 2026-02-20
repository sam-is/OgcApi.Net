using Microsoft.OpenApi;
using OgcApi.Net.Options;
using IOpenApiExtension = OgcApi.Net.OpenApi.Interfaces.IOpenApiExtension;

namespace OgcApi.Net.Schemas;

public class SchemasOpenApiExtension : IOpenApiExtension
{
    public void Apply(OpenApiDocument document, OgcApiOptions ogcApiOptions)
    {
        var defaultSchema = new OpenApiSchema
        {
            Title = "Schema",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["$schema"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "The URI of the JSON Schema that this schema conforms to",
                    Example = "https://json-schema.org/draft/2020-12/schema",
                    Default = "https://json-schema.org/draft/2020-12/schema",
                    Enum = ["https://json-schema.org/draft/2020-12/schema"]
                },
                ["$id"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "A unique identifier for the schema",
                    Example = "https://example.com/ogc/api/ogc/collections/example/schema",
                },
                ["type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Type of the root element",
                    Example = "object",
                    Default = "object",
                    Enum = ["object"]
                },
                ["title"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Human-readable title for the schema"
                },
                ["description"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                },
                ["additionalProperties"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Boolean,
                    Description = "The \"additionalProperties\" member with a value of \"true\" (the default) or \"false\" is used to state the expected behavior with respect to properties that are not explicitly declared in the schema. If \"additionalProperties\" is set to \"false\", properties that are not explicitly declared in the schema SHALL NOT be allowed, otherwise they SHALL be allowed"
                },
                ["properties"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object,
                    Description = "Map of property names to their schema definitions.",
                    Properties = new Dictionary<string, IOpenApiSchema>(),
                    AdditionalPropertiesAllowed = true,
                    AdditionalProperties = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["type"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Example = "string",
                                Enum = ["string", "number", "integer", "boolean", "object", "array"]
                            },
                            ["format"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Example = "date-time"
                            },
                            ["title"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            },
                            ["enum"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Array
                            },
                            ["x-ogc-role"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            },
                            ["x-ogc-propertySeq"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Integer
                            }
                        },
                    }
                }
            },
            Required = new HashSet<string> { "$schema", "$id", "type" }
        };

        document.Components ??= new OpenApiComponents();

        document.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();

        document.Components.Schemas.Add("OgcJsonSchema", defaultSchema);

        foreach (var collection in ogcApiOptions.Collections.Items)
        {
            document.Paths.Add($"/collections/{collection.Id}/schema", new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Get the JSON Schema of the feature collection",
                        Description = "Returns a JSON Schema that describes the structure and metadata of the features in this collection.",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("OgcJsonSchema")
                                    }
                                }
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Not Found",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("ProblemDetails")
                                    }
                                }
                            }
                        }
                    }
                },
            });

            document.Paths.Add($"/collections/{collection.Id}/queryables", new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Get queryable properties of the feature collection",
                        Description = "Returns a list of properties that can be used for filtering features.",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("OgcJsonSchema")
                                    }
                                }
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Not Found",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("ProblemDetails")
                                    }
                                }
                            }
                        }
                    }
                },
            });

            document.Paths.Add($"/collections/{collection.Id}/sortables", new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Get sortable properties of the feature collection",
                        Description = "Returns a list of properties that can be used to sort features.",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("OgcJsonSchema")
                                    }
                                }
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Not Found",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("ProblemDetails")
                                    }
                                }
                            }
                        }
                    }
                },
            });
        }
    }
}