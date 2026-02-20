using Microsoft.OpenApi;
using OgcApi.Net.Options;
using IOpenApiExtension = OgcApi.Net.OpenApi.Interfaces.IOpenApiExtension;

namespace OgcApi.Net.Styles.Extensions;

/// <summary>
/// Populates OpenAPI document with schemas and paths for styles endpoints.
/// </summary>
public class StylesOpenApiExtension : IOpenApiExtension
{
    public void Apply(OpenApiDocument document, OgcApiOptions ogcApiOptions)
    {
        var ogcStyleSchema = new OpenApiSchema
        {
            Title = "OgcStyle",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["id"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Style identifier",
                },
                ["title"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Style title"
                },
                ["links"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchemaReference("Link")
                },
            }
        };

        document.Components ??= new OpenApiComponents();

        document.Components.Schemas ??= new Dictionary<string, IOpenApiSchema>();

        document.Components.Schemas.Add("OgcStyleSchema", ogcStyleSchema);

        var ogcStylesSchema = new OpenApiSchema
        {
            Title = "OgcStyles",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["default"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Default style identifier"
                },
                ["styles"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Description = "Styles list",
                    Items = new OpenApiSchemaReference("OgcStyleSchema")
                }
            }
        };
        document.Components.Schemas.Add("OgcStylesSchema", ogcStylesSchema);

        var ogcStyleMetadata = new OpenApiSchema
        {
            Title = "Style metadata",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["id"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Style identifier"
                },
                ["title"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Title"
                },
                ["description"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Description"
                },
                ["keywords"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Description = "Keywords",
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.String
                    }
                },
                ["pointOfContact"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Point of Contact"
                },
                ["license"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "License"
                },
                ["created"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Created",
                    Format = "date-time"
                },
                ["updated"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Updated",
                    Format = "date-time"
                },
                ["scope"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Scope"
                },
                ["version"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Version"
                },
            }
        };
        document.Components.Schemas.Add("OgcStyleMetadataSchema", ogcStyleMetadata);

        var stylesheetAddParameters = new OpenApiSchema
        {
            Title = "StylesheetAddParameters",
            Description = "Parameters used to add new style for the collection",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["styleId"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Style identifier"
                },
                ["format"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Stylesheet format"
                },
                ["content"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Stylesheet content"
                },

            }
        };
        document.Components.Schemas.Add("StylesheetAddParametersSchema", stylesheetAddParameters);

        var defaultStyle = new OpenApiSchema
        {
            Title = "DefaultStyle",
            Description = "Parameter used to update or retrieve default style for the collection",
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["default"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Description = "Default style identifier"
                }
            }
        };
        document.Components.Schemas.Add("DefaultStyleSchema", defaultStyle);

        foreach (var collection in ogcApiOptions.Collections.Items)
        {
            document.Paths.Add($"/collections/{collection.Id}/styles", new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Gets a list of available styles for the collection",
                        Description = "Returns styles for the collection",
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("OgcStyleSchema")
                                    }
                                },
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error",
                            },
                        }
                    },
                    [HttpMethod.Post] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Adds new stylesheet for the collection",
                        Description = "Adds a new style to the styles storage if style does not exist.",
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("StylesheetAddParametersSchema")
                                }
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Default style not found",
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error"
                            }
                        }
                    },
                    [HttpMethod.Patch] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Updates default style of the collection",
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/merge-patch+json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("DefaultStyleSchema")
                                }
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            ["204"] = new OpenApiResponse
                            {
                                Description = "Created",
                            },
                            ["409"] = new OpenApiResponse
                            {
                                Description = "Style already exists",
                            }
                        }
                    },
                }
            });

            document.Paths.Add($"/collections/{collection.Id}/styles/{{styleId}}", new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Gets a style by its identifier",
                        Description = "Returns style info or a stylesheet if format provided",
                        Parameters =
                        [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                }
                            },
                            new OpenApiParameter
                            {
                                Name = "f",
                                Description = "Stylesheet format (e.g. mapbox, sld10, sld11)",
                                In = ParameterLocation.Query,
                                Required = false,
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                }
                            },
                        ],
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("OgcStyleSchema")
                                    }
                                },
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Not found"
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error",
                            },
                        }
                    },
                    [HttpMethod.Put] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Replaces existing stylesheet",
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("StylesheetAddParametersSchema")
                                }
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Style not found",
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error"
                            }
                        }
                    },
                    [HttpMethod.Delete] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Deletes existing style",
                        Parameters =
                        [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                }
                            },
                        ],
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Style not found",
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error"
                            }
                        }
                    }
                }
            });

            document.Paths.Add($"/collections/{collection.Id}/styles/{{styleId}}/metadata", new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Gets a metadata of the style",
                        Description = "Gets a metadata of the style",
                        Parameters =
                        [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                }
                            }
                        ],
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("OgcStyleMetadataSchema")
                                    }
                                },
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Style not found"
                            },
                        }
                    },
                    [HttpMethod.Put] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Replaces existing metadata of the style with new metadata instance",
                        Parameters =
                        [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                }
                            },
                        ],
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("OgcStyleMetadataSchema")
                                }
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error"
                            }
                        }
                    },
                    [HttpMethod.Patch] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Updates existing metadata",
                        Parameters =
                        [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = JsonSchemaType.String,
                                }
                            },
                        ],
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, IOpenApiMediaType>
                            {
                                ["application/merge-patch+json"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchemaReference("OgcStyleMetadataSchema")
                                }
                            }
                        },
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Style not found"
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error"
                            }
                        }
                    }
                }
            });
        }
    }
}