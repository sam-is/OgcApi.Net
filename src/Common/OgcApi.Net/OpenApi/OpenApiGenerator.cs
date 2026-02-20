using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using OgcApi.Net.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using IOpenApiExtension = OgcApi.Net.OpenApi.Interfaces.IOpenApiExtension;

namespace OgcApi.Net.OpenApi;

public class OpenApiGenerator(IOptionsMonitor<OgcApiOptions> apiOptions, IEnumerable<IOpenApiExtension> extensions) : IOpenApiGenerator
{
    private readonly OgcApiOptions _apiOptions = apiOptions?.CurrentValue ?? throw new ArgumentNullException(nameof(apiOptions));

    public OpenApiDocument GetDocument(Uri baseUrl)
    {
        var openApiDocument = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = _apiOptions.LandingPage.Title,
                Description = _apiOptions.LandingPage.Description,
                Version = _apiOptions.LandingPage.Version,
                Contact = new OpenApiContact
                {
                    Name = _apiOptions.LandingPage.ContactName,
                    Url = _apiOptions.LandingPage.ContactUrl
                },
                License = new OpenApiLicense
                {
                    Name = _apiOptions.LandingPage.LicenseName,
                    Url = _apiOptions.LandingPage.LicenseUrl
                }
            },
            Servers =
            [
                new() { Url = baseUrl.ToString() }
            ],
            Paths = new OpenApiPaths
            {
                ["/"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Get] = new()
                        {
                            Description = "The landing page provides links to the API definition, the conformance statements and to the feature collections in this dataset.",
                            Tags = new HashSet<OpenApiTagReference>
                            {
                                new("Capabilities")
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, IOpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Object,
                                                Properties = new Dictionary<string, IOpenApiSchema>
                                                {
                                                    ["title"] = new OpenApiSchema { Type = JsonSchemaType.String },
                                                    ["description"] = new OpenApiSchema { Type = JsonSchemaType.String },
                                                    ["links"] = new OpenApiSchema
                                                    {
                                                        Type = JsonSchemaType.Array,
                                                        Items = new OpenApiSchemaReference("Link")
                                                    }
                                                },
                                                Required = new HashSet<string> { "links" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                ["/conformance"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Get] = new()
                        {
                            Description = "Information about specifications that this API conforms to",
                            Tags = new HashSet<OpenApiTagReference>
                            {
                                new("Capabilities")
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, IOpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Object,
                                                Properties = new Dictionary<string, IOpenApiSchema>
                                                {
                                                    ["conformsTo"] = new OpenApiSchema
                                                    {
                                                        Type = JsonSchemaType.Array,
                                                        Items = new OpenApiSchema { Type = JsonSchemaType.String }
                                                    }
                                                },
                                                Required = new HashSet<string> { "conformsTo" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                ["/collections"] = new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Get] = new()
                        {
                            Description = "The feature collections in the dataset",
                            Tags = new HashSet<OpenApiTagReference>
                            {
                                new("Capabilities")
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, IOpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Object,
                                                Properties = new Dictionary<string, IOpenApiSchema>
                                                {
                                                    ["links"] = new OpenApiSchema
                                                    {
                                                        Type = JsonSchemaType.Array,
                                                        Items = new OpenApiSchemaReference("Link")
                                                    },
                                                    ["collections"] = new OpenApiSchema
                                                    {
                                                        Type = JsonSchemaType.Array,
                                                        Items = new OpenApiSchemaReference("Collection")
                                                    }
                                                },
                                                Required = new HashSet<string> { "links", "collections" }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Link"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["href"] = new OpenApiSchema { Type = JsonSchemaType.String },
                            ["rel"] = new OpenApiSchema { Type = JsonSchemaType.String },
                            ["type"] = new OpenApiSchema { Type = JsonSchemaType.String },
                            ["hreflang"] = new OpenApiSchema { Type = JsonSchemaType.String },
                            ["title"] = new OpenApiSchema { Type = JsonSchemaType.String },
                            ["length"] = new OpenApiSchema { Type = JsonSchemaType.String },
                        },
                        Required = new HashSet<string> { "href", "rel" }
                    },
                    ["Collection"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["id"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "An identifier of the collection used, for example, in URIs"
                            },
                            ["title"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "A human-readable title of the collection"
                            },
                            ["description"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "A description of the features in the collection"
                            },
                            ["links"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Array,
                                Items = new OpenApiSchemaReference("Link")
                            },
                            ["extent"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Object,
                                Description = "The extent of the features in the collection",
                                Properties = new Dictionary<string, IOpenApiSchema>
                                {
                                    ["spatial"] = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Object,
                                        Description = "The spatial extent of the features in the collection",
                                        Properties = new Dictionary<string, IOpenApiSchema>
                                        {
                                            ["bbox"] = new OpenApiSchema
                                            {
                                                Description = "One or more bounding boxes that describe the spatial extent of the dataset",
                                                Type = JsonSchemaType.Array,
                                                MinItems = 1,
                                                Items = new OpenApiSchema
                                                {
                                                    Description = "Each bounding box is provided as four or six numbers, depending on\n" +
                                                                  " whether the coordinate reference system includes a vertical axis\n" +
                                                                  "  (height or depth):\n\n" +
                                                                  "* Lower left corner, coordinate axis 1\n" +
                                                                  "* Lower left corner, coordinate axis 2\n" +
                                                                  "* Upper right corner, coordinate axis 1\n" +
                                                                  "* Upper right corner, coordinate axis 2\n\n" +
                                                                  "The coordinate reference system of the values is WGS 84 longitude/latitude\n" +
                                                                  "(http://www.opengis.net/def/crs/OGC/1.3/CRS84) unless a different ordinate\n" +
                                                                  "reference system is specified in `crs`.\n" +
                                                                  "For WGS 84 longitude/latitude the values are in most cases the sequence of\n" +
                                                                  "minimum longitude, minimum latitude, maximum longitude and maximum latitude.\n" +
                                                                  "However, in cases where the box spans the antimeridian the first value\n" +
                                                                  "(west-most box edge) is larger than the third value (east-most box edge).\n" +
                                                                  "If the vertical axis is included, the third and the sixth number are\n" +
                                                                  "the bottom and the top of the 3-dimensional bounding box.\n" +
                                                                  "If a feature has multiple spatial geometry properties, it is the decision of the\n" +
                                                                  "server whether only a single spatial geometry property is used to determine\n" +
                                                                  "the extent or all relevant geometries.",
                                                    Type = JsonSchemaType.Array,
                                                    MinItems = 4,
                                                    MaxItems = 6,
                                                    Items = new OpenApiSchema
                                                    {
                                                        Type = JsonSchemaType.Number
                                                    },
                                                    Example = new JsonArray
                                                    {
                                                        -180, -90, 180, 90
                                                    }
                                                }
                                            },
                                            ["crs"] = new OpenApiSchema
                                            {
                                                Description = "Coordinate reference system of the coordinates in the spatial extent\n" +
                                                              "(property `bbox`). The default reference system is WGS 84 longitude/latitude",
                                                Type = JsonSchemaType.String,
                                                Default = "http://www.opengis.net/def/crs/OGC/1.3/CRS84",

                                            }
                                        }
                                    },
                                    ["temporal"] = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Object,
                                        Description = "The temporal extent of the features in the collection",
                                        Properties = new Dictionary<string, IOpenApiSchema>
                                        {
                                            ["interval"] = new OpenApiSchema
                                            {
                                                Description = "One or more time intervals that describe the temporal extent of the dataset.\n" +
                                                              "The value `null` is supported and indicates an open time interval",
                                                Type = JsonSchemaType.Array,
                                                MinItems = 1,
                                                Items = new OpenApiSchema
                                                {
                                                    Description = "Begin and end times of the time interval. The timestamps\n" +
                                                                  "are in the coordinate reference system specified in `trs`. By default\n" +
                                                                  "this is the Gregorian calendar.",
                                                    Type = JsonSchemaType.Array,
                                                    MinItems = 2,
                                                    MaxItems = 2,
                                                    Items = new OpenApiSchema
                                                    {
                                                        Type = JsonSchemaType.String | JsonSchemaType.Null,
                                                        Format = "date-time",
                                                        Example = new JsonArray
                                                        {
                                                            "2011-11-11T12:22:11Z",
                                                            null
                                                        }
                                                    }
                                                }
                                            },
                                            ["trs"] = new OpenApiSchema
                                            {
                                                Description = "Coordinate reference system of the coordinates in the temporal extent\n" +
                                                              "(property `interval`). The default reference system is the Gregorian calendar.\n" +
                                                              "In the Core this is the only supported temporal reference system.\n" +
                                                              "Extensions may support additional temporal reference systems and add\n" +
                                                              "additional enum values.",
                                                Type = JsonSchemaType.String,
                                                Default = "http://www.opengis.net/def/uom/ISO-8601/0/Gregorian"
                                            }
                                        }
                                    }
                                }
                            },
                            ["itemType"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Description = "An indicator about the type of the items in the collection (the default value is 'feature')",
                                Default = "feature"
                            },
                            ["crs"] = new OpenApiSchema
                            {
                                Description = "The list of coordinate reference systems supported by the service",
                                Type = JsonSchemaType.Array,
                                Items = new OpenApiSchema { Type = JsonSchemaType.String },
                                Default = new JsonArray { "http://www.opengis.net/def/crs/OGC/1.3/CRS84" }
                            }
                        },
                        Required = new HashSet<string> { "id", "links" }
                    },
                    ["ProblemDetails"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["type"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String | JsonSchemaType.Null
                            },
                            ["title"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String | JsonSchemaType.Null
                            },
                            ["status"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Integer | JsonSchemaType.Null,
                                Format = "int32"
                            },
                            ["detail"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String | JsonSchemaType.Null
                            },
                            ["instance"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String | JsonSchemaType.Null
                            }
                        }
                    },
                    ["Tileset"] = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema>
                        {
                            ["title"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            },
                            ["tileMatrixSetURI"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            },
                            ["crs"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String
                            },
                            ["dataType"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.String,
                                Enum = ["vector"]
                            },
                            ["links"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Array,
                                Items = new OpenApiSchemaReference("Link")
                            },
                            ["tileMatrixSetLimits"] = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Array,
                                Items = new OpenApiSchema
                                {
                                    MinItems = 1,
                                    Type = JsonSchemaType.Object,
                                    Properties = new Dictionary<string, IOpenApiSchema>
                                    {
                                        ["tileMatrix"] = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int32"
                                        },
                                        ["minTileRow"] = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int32"
                                        },
                                        ["maxTileRow"] = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int32"
                                        },
                                        ["minTileCol"] = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int32"
                                        },
                                        ["maxTileCol"] = new OpenApiSchema
                                        {
                                            Type = JsonSchemaType.Integer,
                                            Format = "int32"
                                        }
                                    }
                                }
                            }
                        },
                        Required = new HashSet<string> { "tileMatrixSetURI", "crs", "dataType", "links", "tileMatrixSetLimits" }
                    }
                }
            }
        };

        foreach (var collection in _apiOptions.Collections.Items)
        {
            openApiDocument.Paths.Add($"/collections/{collection.Id}", new OpenApiPathItem
            {
                Operations = new Dictionary<HttpMethod, OpenApiOperation>
                {
                    [HttpMethod.Get] = new()
                    {
                        Tags = new HashSet<OpenApiTagReference>
                        {
                            new(collection.Title)
                        },
                        Summary = "Feature collection metadata",
                        Description = collection.Description,
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new OpenApiResponse
                            {
                                Description = "Success",
                                Content = new Dictionary<string, IOpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = new OpenApiSchemaReference("Collection")
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
                }
            });

            if (collection.Features != null)
            {
                openApiDocument.Paths.Add($"/collections/{collection.Id}/items", new OpenApiPathItem
                {
                    Operations = GetFeatureCollectionOperations(collection)
                });

                openApiDocument.Paths.Add($"/collections/{collection.Id}/items/{{featureId}}", new OpenApiPathItem
                {
                    Operations = GetFeatureOperations(collection)
                });
            }

            if (collection.Tiles != null)
            {
                openApiDocument.Paths.Add($"/collections/{collection.Id}/tiles", new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Get] = new()
                        {
                            Tags = new HashSet<OpenApiTagReference>
                            {
                                new(collection.Title)
                            },
                            Summary = "Provides a list of available tilesets for a resource",
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new OpenApiResponse
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, IOpenApiMediaType>
                                    {
                                        ["application/json"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.Array,
                                                Items = new OpenApiSchemaReference("Tileset")
                                            }
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
                    }
                });

                openApiDocument.Paths.Add($"/collections/{collection.Id}/tiles/{{tileMatrix}}/{{tileRow}}/{{tileCol}}", new OpenApiPathItem
                {
                    Operations = new Dictionary<HttpMethod, OpenApiOperation>
                    {
                        [HttpMethod.Get] = new()
                        {
                            Tags = new HashSet<OpenApiTagReference>
                            {
                                new(collection.Title)
                            },
                            Summary = "Retrieve the vector tile for the specified tile matrix, row, and column",
                            Parameters =
                            [
                                new OpenApiParameter
                                {
                                    Name = "tileMatrix",
                                    Description = "Identifier of the tile matrix (representing a zoom level, a.k.a. a scale) listed in the TileMatrixSet definition",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Integer,
                                        Format = "int32"
                                    }
                                },
                                new OpenApiParameter
                                {
                                    Name = "tileRow",
                                    Description = "A non-negative integer between 0 and the MatrixHeight - 1. If there is a TileMatrixSetLimits the value is limited between MinTileRow and MaxTileRow",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Integer,
                                        Format = "int32"
                                    }
                                },
                                new OpenApiParameter
                                {
                                    Name = "tileCol",
                                    Description = "A non-negative integer between 0 and the MatrixWidth - 1. If there is a TileMatrixSetLimits the value is limited between MinTileCol and MaxTileCol",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema
                                    {
                                        Type = JsonSchemaType.Integer,
                                        Format = "int32"
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
                                        ["application/vnd.mapbox-vector-tile"] = new OpenApiMediaType
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = JsonSchemaType.String,
                                                Format = "binary"
                                            }
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
                    }
                });
            }
        }

        if (_apiOptions.UseApiKeyAuthorization)
        {
            openApiDocument.Security ??= [];

            openApiDocument.Security.Add(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("ApiKey", openApiDocument), []
                }
            });

            openApiDocument.AddComponent("ApiKey", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Query,
                Name = "apiKey",
                Description = "API key"
            });
        }

        foreach (var extension in extensions)
            extension.Apply(openApiDocument, _apiOptions);

        return openApiDocument;
    }

    private static OpenApiSchema GetFeatureCollectionSchema(CollectionOptions collectionOptions)
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "features" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema { Enum = ["FeatureCollection"] },
                ["features"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = GetFeatureSchema(collectionOptions)
                },
                ["links"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchemaReference("Link")
                },
                ["timeStamp"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Format = "date-time"
                },
                ["numberMatched"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Integer,
                    Minimum = "0"
                },
                ["numberReturned"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Integer,
                    Minimum = "0"
                }
            }
        };
    }

    private static OpenApiSchema GetFeatureSchema(CollectionOptions collectionOptions)
    {
        var collectionSourceOptions = collectionOptions.Features.Storage;

        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "geometry", "properties" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema { Enum = ["Feature"] },
                ["geometry"] = GetGeometrySchema(collectionSourceOptions?.GeometryGeoJsonType),
                ["properties"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Object | JsonSchemaType.Null,
                    Properties = collectionSourceOptions?.Properties?.ToDictionary(key => key, _ => new OpenApiSchema() as IOpenApiSchema)
                },
                ["id"] = new OpenApiSchema
                {
                    OneOf =
                    [
                        new OpenApiSchema { Type = JsonSchemaType.String },
                        new OpenApiSchema { Type = JsonSchemaType.Integer }
                    ]
                },
                ["links"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchemaReference("Link")
                }
            }
        };
    }

    private static OpenApiSchema GetGeometrySchema(string geometryType)
    {
        return geometryType switch
        {
            "Point" => GetPointSchema(),
            "MultiPoint" => new OpenApiSchema
            {
                AnyOf =
                [
                    GetPointSchema(),
                    GetMultiPointSchema()
                ]
            },
            "LineString" => GetLineStringSchema(),
            "MultiLineString" => new OpenApiSchema
            {
                AnyOf =
                [
                    GetLineStringSchema(),
                    GetMultiLineStringSchema()
                ]
            },
            "Polygon" => GetPolygonSchema(),
            "MultiPolygon" => new OpenApiSchema
            {
                AnyOf =
                [
                    GetPolygonSchema(),
                    GetMultiPolygonSchema()
                ]
            },
            _ => null
        };
    }

    private static OpenApiSchema GetPointSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = ["Point"]
                },
                ["coordinates"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    MinItems = 2,
                    Items = new OpenApiSchema { Type = JsonSchemaType.Number }
                }
            }
        };
    }

    private static OpenApiSchema GetMultiPointSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = ["MultiPoint"]
                },
                ["coordinates"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        MinItems = 2,
                        Items = new OpenApiSchema { Type = JsonSchemaType.Number }
                    }
                }
            }
        };
    }

    private static OpenApiSchema GetLineStringSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = ["LineString"]
                },
                ["coordinates"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    MinItems = 2,
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        MinItems = 2,
                        Items = new OpenApiSchema { Type = JsonSchemaType.Number }
                    }
                }
            }
        };
    }

    private static OpenApiSchema GetMultiLineStringSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = ["MultiLineString"]
                },
                ["coordinates"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        MinItems = 2,
                        Items = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            MinItems = 2,
                            Items = new OpenApiSchema { Type = JsonSchemaType.Number }
                        }
                    }
                }
            }
        };
    }

    private static OpenApiSchema GetPolygonSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = ["Polygon"]
                },
                ["coordinates"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        MinItems = 4,
                        Items = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            MinItems = 2,
                            Items = new OpenApiSchema { Type = JsonSchemaType.Number }
                        }
                    }
                }
            }
        };
    }

    private static OpenApiSchema GetMultiPolygonSchema()
    {
        return new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["type"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.String,
                    Enum = ["MultiPolygon"]
                },
                ["coordinates"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema
                    {
                        Type = JsonSchemaType.Array,
                        Items = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            MinItems = 4,
                            Items = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Array,
                                MinItems = 2,
                                Items = new OpenApiSchema { Type = JsonSchemaType.Number }
                            }
                        }
                    }
                }
            }
        };
    }

    private static Dictionary<HttpMethod, OpenApiOperation> GetFeatureOperations(CollectionOptions collection)
    {
        var result = new Dictionary<HttpMethod, OpenApiOperation>
        {
            [HttpMethod.Get] = new()
            {
                Tags = new HashSet<OpenApiTagReference>
                {
                    new(collection.Title)
                },
                Summary = "Fetch feature",
                Description = $"Fetch the feature with id featureId in the feature collection with id {collection.Id}.",
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                    },
                    new OpenApiParameter
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the response document will be presented in the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "uri"
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
                            ["application/geo+json"] = new OpenApiMediaType
                            {
                                Schema = GetFeatureSchema(collection)
                            }
                        }
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
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
        };

        if (collection.Features.Storage.AllowReplace)
        {
            result.Add(HttpMethod.Put, new OpenApiOperation
            {
                Tags = new HashSet<OpenApiTagReference>
                {
                    new(collection.Title)
                },
                Summary = "Replace feature",
                Description = "Replace an existing resource",
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature to replace",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                    },
                    new OpenApiParameter
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the request document will be converted from the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "uri"
                        }
                    }
                ],
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, IOpenApiMediaType>
                    {
                        ["application/geo+json"] = new OpenApiMediaType
                        {
                            Schema = GetFeatureSchema(collection)
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Success"
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
                            }
                        }
                    },
                    ["401"] = new OpenApiResponse
                    {
                        Description = "Unauthorized",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
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
            });
        }

        if (collection.Features.Storage.AllowDelete)
        {
            result.Add(HttpMethod.Delete, new OpenApiOperation
            {
                Tags = new HashSet<OpenApiTagReference>
                {
                    new(collection.Title)
                },
                Summary = "Delete feature",
                Description = "Remove a resource from a collection",
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature to delete",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                    }
                ],
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Success"
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
                            }
                        }
                    },
                    ["401"] = new OpenApiResponse
                    {
                        Description = "Unauthorized",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
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
            });
        }

        if (collection.Features.Storage.AllowUpdate)
        {
            result.Add(HttpMethod.Patch, new OpenApiOperation
            {
                Tags = new HashSet<OpenApiTagReference>
                {
                    new(collection.Title)
                },
                Summary = "Update feature",
                Description = "Modify an existing resource",
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature to update",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = JsonSchemaType.String }
                    },
                    new OpenApiParameter
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the request document will be converted from the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "uri"
                        }
                    }
                ],
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, IOpenApiMediaType>
                    {
                        ["application/geo+json"] = new OpenApiMediaType
                        {
                            Schema = GetFeatureSchema(collection)
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Success"
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
                            }
                        }
                    },
                    ["401"] = new OpenApiResponse
                    {
                        Description = "Unauthorized",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
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
            });
        }

        return result;
    }

    private static Dictionary<HttpMethod, OpenApiOperation> GetFeatureCollectionOperations(CollectionOptions collection)
    {
        var result = new Dictionary<HttpMethod, OpenApiOperation>
        {
            [HttpMethod.Get] = new()
            {
                Tags = new HashSet<OpenApiTagReference>
                {
                    new(collection.Title)
                },
                Summary = "Fetch features",
                Description = $"Fetch features of the feature collection with id {collection.Id}.\n" +
                              "Every feature in a dataset belongs to a collection. A dataset may consist of multiple feature collections. A feature collection is often a collection of features of a similar type, based on a common schema.",
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "limit",
                        Description = "Limits the number of items that are presented in the response document",
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form,
                        Explode = false,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int32",
                            Default = 10,
                            Minimum = "1",
                            Maximum = "10000"
                        }
                    },
                    new OpenApiParameter
                    {
                        Name = "offset",
                        Description = "Offset for requesting objects. The resulting response will contain a link for the next features page",
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Integer,
                            Format = "int32",
                            Default = 0
                        }
                    },
                    new OpenApiParameter
                    {
                        Name = "bbox",
                        Description = @"Only features that have a geometry that intersects the bounding box are selected.\n" +
                                      "The bounding box is provided as four or six numbers, depending on whether the\n" +
                                      "coordinate reference system includes a vertical axis (height or depth):\n\n" +
                                      "* Lower left corner, coordinate axis 1\n" +
                                      "* Lower left corner, coordinate axis 2\n" +
                                      "* Upper right corner, coordinate axis 1\n" +
                                      "* Upper right corner, coordinate axis 2\n\n" +
                                      "The coordinate reference system of the values is WGS 84 longitude/latitude\n" +
                                      "(http://www.opengis.net/def/crs/OGC/1.3/CRS84) unless a different coordinate\n" +
                                      "reference system is specified in the parameter `bbox-crs`.\n\n" +
                                      "For WGS 84 longitude/latitude the values are in most cases the sequence of\n" +
                                      "minimum longitude, minimum latitude, maximum longitude and maximum latitude.\n" +
                                      "However, in cases where the box spans the antimeridian the first value\n" +
                                      "(west-most box edge) is larger than the third value (east-most box edge).\n\n" +
                                      "If the vertical axis is included, the third and the sixth number are\n" +
                                      "the bottom and the top of the 3-dimensional bounding box.\n\n" +
                                      "If a feature has multiple spatial geometry properties, it is the decision of the\n" +
                                      "server whether only a single spatial geometry property is used to determine\n" +
                                      "the extent or all relevant geometries.",
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form,
                        Explode = false,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.Array,
                            MinItems = 4,
                            MaxItems = 6,
                            Items = new OpenApiSchema
                            {
                                Type = JsonSchemaType.Number
                            }
                        }
                    },
                    new OpenApiParameter
                    {
                        Name = "bbox-crs",
                        Description = "Parameter may be used to assert the CRS used for the coordinate values of the bbox parameter. " +
                                      "If the bbox-crs parameter is not specified then the coordinate values of the bbox parameter is the default CRS that is http://www.opengis.net/def/crs/OGC/1.3/CRS84",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "uri"
                        }
                    },
                    new OpenApiParameter
                    {
                        Name = "datetime",
                        Description = "Either a date-time or an interval, open or closed. Date and time expressions\n" +
                                      "adhere to RFC 3339. Open intervals are expressed using double-dots.\n\n" +
                                      "Examples:\n\n" +
                                      "* A date-time: \"2018-02-12T23:20:50Z\"\n" +
                                      "* A closed interval: \"2018-02-12T00:00:00Z/2018-03-18T12:31:12Z\"\n" +
                                      "* Open intervals: \"2018-02-12T00:00:00Z/..\" or \"../2018-03-18T12:31:12Z\"\n\n" +
                                      "Only features that have a temporal property that intersects the value of\n" +
                                      "`datetime` are selected.\n\n" +
                                      "If a feature has multiple temporal properties, it is the decision of the\n" +
                                      "server whether only a single temporal property is used to determine\n" +
                                      "the extent or all relevant temporal properties.",
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form,
                        Explode = false,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String
                        }
                    },
                    new OpenApiParameter
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the response document will be presented in the requested CRS",
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "uri"
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
                            ["application/geo+json"] = new OpenApiMediaType
                            {
                                Schema = GetFeatureCollectionSchema(collection)
                            }
                        }
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
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
        };

        if (collection.Features.Storage.AllowCreate)
        {
            result.Add(HttpMethod.Post, new OpenApiOperation
            {
                Tags = new HashSet<OpenApiTagReference>
                {
                    new(collection.Title)
                },
                Summary = "Create feature",
                Description = "Add a new resource to a collection",
                Parameters =
                [
                    new OpenApiParameter
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the request document will be converted from the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = JsonSchemaType.String,
                            Format = "uri"
                        }
                    }
                ],
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, IOpenApiMediaType>
                    {
                        ["application/geo+json"] = new OpenApiMediaType
                        {
                            Schema = GetFeatureSchema(collection)
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["201"] = new OpenApiResponse
                    {
                        Description = "Success",
                        Headers = new Dictionary<string, IOpenApiHeader>
                        {
                            ["Location"] = new OpenApiHeader()
                        }
                    },
                    ["400"] = new OpenApiResponse
                    {
                        Description = "Bad Request",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
                            }
                        }
                    },
                    ["401"] = new OpenApiResponse
                    {
                        Description = "Unauthorized",
                        Content = new Dictionary<string, IOpenApiMediaType>
                        {
                            ["application/json"] = new OpenApiMediaType
                            {
                                Schema = new OpenApiSchemaReference("ProblemDetails")
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
            });
        }

        return result;
    }
}