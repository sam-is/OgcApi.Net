using Microsoft.OpenApi.Models;
using OgcApi.Net.OpenApi.Interfaces;
using OgcApi.Net.Options;

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
            Properties =
            {
                ["id"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Style identifier",
                },
                ["title"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Style title"
                },
                ["links"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference { Id = "Link", Type = ReferenceType.Schema }
                    }
                },
            }
        };
        document.Components.Schemas.Add("OgcStyleSchema", ogcStyleSchema);

        var ogcStylesSchema = new OpenApiSchema
        {
            Title = "OgcStyles",
            Properties =
            {
                ["default"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Default style identifier"
                },
                ["styles"] = new OpenApiSchema
                {
                    Type = "array",
                    Description = "Styles list",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference {Id = "OgcStyleSchema", Type = ReferenceType.Schema }
                    }
                }
            }
        };
        document.Components.Schemas.Add("OgcStylesSchema", ogcStylesSchema);

        var ogcStyleMetadata = new OpenApiSchema
        {
            Title = "Style metadata",
            Properties =
            {
                ["id"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Style identifier"
                },
                ["title"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Title"
                },
                ["description"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Description"
                },
                ["keywords"] = new OpenApiSchema
                {
                    Type = "array",
                    Description = "Keywords",
                    Items = new OpenApiSchema
                    {
                        Type = "string"
                    }
                },
                ["pointOfContact"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Point of Contact"
                },
                ["license"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "License"
                },
                ["created"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Created",
                    Format = "date-time"
                },
                ["updated"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Updated",
                    Format = "date-time"
                },
                ["scope"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Scope"
                },
                ["version"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Version"
                },
            }
        };
        document.Components.Schemas.Add("OgcStyleMetadataSchema", ogcStyleMetadata);

        var stylesheetAddParameters= new OpenApiSchema
        {
            Title = "StylesheetAddParameters",
            Description = "Parameters used to add new style for the collection",
            Properties =
            {
                ["styleId"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Style identifier"
                },
                ["format"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Stylesheet format"
                },
                ["content"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Stylesheet content"
                },
                
            }
        };
        document.Components.Schemas.Add("StylesheetAddParametersSchema", stylesheetAddParameters);

        var defaultStyle = new OpenApiSchema
        {
            Title = "DefaultStyle",
            Description = "Parameter used to update or retrieve default style for the collection",
            Properties =
            {
                ["default"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "Default style identifier"
                }
            }
        };
        document.Components.Schemas.Add("DefaultStyleSchema", defaultStyle);

        foreach (var collection in ogcApiOptions.Collections.Items)
        {
            document.Paths.Add($"/collections/{collection.Id}/styles", new OpenApiPathItem
            {
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title }
                        ],
                        Summary = "Gets a list of available styles for the collection",
                        Description = "Returns styles for the collection",
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
                                            Reference = new OpenApiReference { Id = "OgcStylesSchema", Type = ReferenceType.Schema }
                                        }
                                    }
                                },
                            },
                            ["500"] = new OpenApiResponse
                            {
                                Description = "Internal server error",
                            },
                        }
                    },
                    [OperationType.Post] = new()
                    {
                        Tags =
                        [
                          new OpenApiTag { Name = collection.Title}  
                        ],
                        Summary = "Adds new stylesheet for the collection",
                        Description = "Adds a new style to the styles storage if style does not exist.",
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new()
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference { Id = "StylesheetAddParametersSchema", Type = ReferenceType.Schema }
                                    }
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
                    [OperationType.Patch] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title}
                        ],
                        Summary = "Updates default style of the collection",
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/merge-patch+json"] = new()
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference { Id = "DefaultStyleSchema", Type = ReferenceType.Schema }
                                    }
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
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title }
                        ],
                        Summary = "Gets a style by its identifier",
                        Description = "Returns style info or a stylesheet if format provided",
                        Parameters = [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = "string",
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
                                    Type = "string",
                                }
                            },
                        ],
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
                                            Reference = new OpenApiReference { Id = "OgcStyleSchema", Type = ReferenceType.Schema }
                                        }
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
                    [OperationType.Put] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title}
                        ],
                        Summary = "Replaces existing stylesheet",
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new()
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference { Id = "StylesheetAddParametersSchema", Type = ReferenceType.Schema }
                                    }
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
                    [OperationType.Delete] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title}
                        ],
                        Summary = "Deletes existing style",
                        Parameters = [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = "string",
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
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title }
                        ],
                        Summary = "Gets a metadata of the style",
                        Description = "Gets a metadata of the style",
                        Parameters = [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = "string",
                                }
                            }
                        ],
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
                                            Reference = new OpenApiReference { Id = "OgcStyleMetadataSchema", Type = ReferenceType.Schema }
                                        }
                                    }
                                },
                            },
                            ["404"] = new OpenApiResponse
                            {
                                Description = "Style not found"
                            },
                        }
                    },
                    [OperationType.Put] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title}
                        ],
                        Summary = "Replaces existing metadata of the style with new metadata instance",
                        Parameters = [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = "string",
                                }
                            },
                        ],
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/json"] = new()
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference { Id = "OgcStyleMetadataSchema", Type = ReferenceType.Schema }
                                    }
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
                    [OperationType.Patch] = new()
                    {
                        Tags =
                        [
                            new OpenApiTag { Name = collection.Title}
                        ],
                        Summary = "Updates existing metadata",
                        Parameters = [
                            new OpenApiParameter
                            {
                                Name = "styleId",
                                Description = "Style identifier",
                                In = ParameterLocation.Path,
                                Required = true,
                                Schema = new OpenApiSchema
                                {
                                    Type = "string",
                                }
                            },
                        ],
                        RequestBody = new OpenApiRequestBody
                        {
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["application/merge-patch+json"] = new()
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Reference = new OpenApiReference { Id = "OgcStyleMetadataSchema", Type = ReferenceType.Schema }
                                    }
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