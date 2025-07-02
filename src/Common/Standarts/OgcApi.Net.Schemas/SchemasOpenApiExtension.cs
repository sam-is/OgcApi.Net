using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OgcApi.Net.OpenApi.Interfaces;
using OgcApi.Net.Options;

namespace OgcApi.Net.Schemas;

public class SchemasOpenApiExtension : IOpenApiExtension
{
    public void Apply(OpenApiDocument document, OgcApiOptions ogcApiOptions)
    {
        var defaultSchema = new OpenApiSchema
        {
            Title = "Schema",
            Properties =
            {
                ["$schema"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "The URI of the JSON Schema that this schema conforms to",
                    Example = new OpenApiString("https://json-schema.org/draft/2020-12/schema"),
                    Default = new OpenApiString("https://json-schema.org/draft/2020-12/schema"),
                    Enum = [new OpenApiString("https://json-schema.org/draft/2020-12/schema")]
                },
                ["$id"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "A unique identifier for the schema",
                    Example = new OpenApiString("https://example.com/ogc/api/ogc/collections/example/schema"),
                },
                ["type"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Type of the root element",
                    Example = new OpenApiString("object"),
                    Default = new OpenApiString("object"),
                    Enum = [new OpenApiString("object")]
                },
                ["title"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Human-readable title for the schema"
                },
                ["description"] = new OpenApiSchema
                {
                    Type = "string",
                },
                ["additionalProperties"] = new OpenApiSchema
                {
                    Type = "boolean",
                    Description = "The \"additionalProperties\" member with a value of \"true\" (the default) or \"false\" is used to state the expected behavior with respect to properties that are not explicitly declared in the schema. If \"additionalProperties\" is set to \"false\", properties that are not explicitly declared in the schema SHALL NOT be allowed, otherwise they SHALL be allowed"
                },
                ["properties"] = new OpenApiSchema
                {
                    Type = "object",
                    Description = "Map of property names to their schema definitions.",
                    Properties = new Dictionary<string, OpenApiSchema>(),
                    AdditionalPropertiesAllowed = true,
                    AdditionalProperties = new OpenApiSchema
                    {
                        Type = "object",
                        Properties =
                        {
                            ["type"] = new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("string"),
                                Enum =
                                [
                                    new OpenApiString("string"),
                                    new OpenApiString("number"),
                                    new OpenApiString("integer"),
                                    new OpenApiString("boolean"),
                                    new OpenApiString("object"),
                                    new OpenApiString("array")
                                ]
                            },
                            ["format"] = new OpenApiSchema
                            {
                                Type = "string",
                                Example = new OpenApiString("date-time")
                            },
                            ["title"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["enum"] = new OpenApiSchema
                            {
                                Type = "array"
                            },
                            ["x-ogc-role"] = new OpenApiSchema
                            {
                                Type = "string"
                            },
                            ["x-ogc-propertySeq"] = new OpenApiSchema
                            {
                                Type = "integer"
                            }
                        },
                    }
                }
            },
            Required = new HashSet<string> { "$schema", "$id", "type" }
        };

        document.Components.Schemas.Add("OgcJsonSchema", defaultSchema);

        foreach (var collection in ogcApiOptions.Collections.Items)
        {
            document.Paths.Add($"/collections/{collection.Id}/schema", new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title }
                        ],
                        Summary = "Get the JSON Schema of the feature collection",
                        Description = "Returns a JSON Schema that describes the structure and metadata of the features in this collection.",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference { Id = "OgcJsonSchema", Type = ReferenceType.Schema }
                                        }
                                    }
                                }
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Not Found",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference { Id = "ProblemDetails", Type = ReferenceType.Schema }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
            });

            document.Paths.Add($"/collections/{collection.Id}/queryables", new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title }
                        ],
                        Summary = "Get queryable properties of the feature collection",
                        Description = "Returns a list of properties that can be used for filtering features.",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference { Id = "OgcJsonSchema", Type = ReferenceType.Schema }
                                        }
                                    }
                                }
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Not Found",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference { Id = "ProblemDetails", Type = ReferenceType.Schema }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
            });

            document.Paths.Add($"/collections/{collection.Id}/sortables", new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title }
                        ],
                        Summary = "Get sortable properties of the feature collection",
                        Description = "Returns a list of properties that can be used to sort features.",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference { Id = "OgcJsonSchema", Type = ReferenceType.Schema }
                                        }
                                    }
                                }
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Not Found",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference { Id = "ProblemDetails", Type = ReferenceType.Schema }
                                        }
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