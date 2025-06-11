using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OgcApi.Net.OpenApi.Interfaces;
using OgcApi.Net.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

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
            Servers = new List<OpenApiServer>
            {
                new() { Url = baseUrl.ToString() }
            },
            Paths = new OpenApiPaths
            {
                ["/"] = new()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            Description = "The landing page provides links to the API definition, the conformance statements and to the feature collections in this dataset.",
                            Tags = new List<OpenApiTag>
                            {
                                new() { Name = "Capabilities" }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new()
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new()
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "object",
                                                Properties = new Dictionary<string, OpenApiSchema>
                                                {
                                                    ["title"] = new() { Type = "string" },
                                                    ["description"] = new() { Type = "string" },
                                                    ["links"] = new()
                                                    {
                                                        Type = "array",
                                                        Items = new OpenApiSchema
                                                        {
                                                            Reference = new OpenApiReference { Id = "Link", Type = ReferenceType.Schema }
                                                        }
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
                ["/conformance"] = new()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            Description = "Information about specifications that this API conforms to",
                            Tags = new List<OpenApiTag>
                            {
                                new() { Name = "Capabilities" }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new()
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new()
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "object",
                                                Properties = new Dictionary<string, OpenApiSchema>
                                                {
                                                    ["conformsTo"] = new()
                                                    {
                                                        Type = "array",
                                                        Items = new OpenApiSchema { Type = "string" }
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
                ["/collections"] = new()
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            Description = "The feature collections in the dataset",
                            Tags = new List<OpenApiTag>
                            {
                                new() { Name = "Capabilities" }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new()
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new()
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "object",
                                                Properties = new Dictionary<string, OpenApiSchema>
                                                {
                                                    ["links"] = new()
                                                    {
                                                        Type = "array",
                                                        Items = new OpenApiSchema
                                                        {
                                                            Reference = new OpenApiReference { Id = "Link", Type = ReferenceType.Schema }
                                                        }
                                                    },
                                                    ["collections"] = new()
                                                    {
                                                        Type = "array",
                                                        Items = new OpenApiSchema
                                                        {
                                                            Reference = new OpenApiReference { Id = "Collection", Type = ReferenceType.Schema }
                                                        }
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
                Schemas = new Dictionary<string, OpenApiSchema>
                {
                    ["Link"] = new()
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["href"] = new() { Type = "string" },
                            ["rel"] = new() { Type = "string" },
                            ["type"] = new() { Type = "string" },
                            ["hreflang"] = new() { Type = "string" },
                            ["title"] = new() { Type = "string" },
                            ["length"] = new() { Type = "string" },
                        },
                        Required = new HashSet<string> { "href" }
                    },
                    ["Collection"] = new()
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["id"] = new() { Type = "string", Description = "identifier of the collection used, for example, in URIs" },
                            ["title"] = new() { Type = "string", Description = "human readable title of the collection" },
                            ["description"] = new() { Type = "string", Description = "a description of the features in the collection" },
                            ["links"] = new()
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference { Id = "Link", Type = ReferenceType.Schema }
                                }
                            },
                            ["extent"] = new()
                            {
                                Type = "object",
                                Description = "The extent of the features in the collection",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["spatial"] = new()
                                    {
                                        Type = "object",
                                        Description = "The spatial extent of the features in the collection",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["bbox"] = new()
                                            {
                                                Description = "One or more bounding boxes that describe the spatial extent of the dataset",
                                                Type = "array",
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
                                                    Type = "array",
                                                    MinItems = 4,
                                                    MaxItems = 6,
                                                    Items = new OpenApiSchema
                                                    {
                                                        Type = "number"
                                                    },
                                                    Example = new OpenApiArray
                                                    {
                                                        new OpenApiDouble(-180),
                                                        new OpenApiDouble(-90),
                                                        new OpenApiDouble(180),
                                                        new OpenApiDouble(90)
                                                    }
                                                }
                                            },
                                            ["crs"] = new()
                                            {
                                                Description = "Coordinate reference system of the coordinates in the spatial extent\n" +
                                                              "(property `bbox`). The default reference system is WGS 84 longitude/latitude",
                                                Type = "string",
                                                Default = new OpenApiString("http://www.opengis.net/def/crs/OGC/1.3/CRS84")
                                            }
                                        }
                                    },
                                    ["temporal"] = new()
                                    {
                                        Type = "object",
                                        Description = "The temporal extent of the features in the collection",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["interval"] = new()
                                            {
                                                Description = "One or more time intervals that describe the temporal extent of the dataset.\n" +
                                                              "The value `null` is supported and indicates an open time interval",
                                                Type = "array",
                                                MinItems = 1,
                                                Items = new OpenApiSchema
                                                {
                                                    Description = "Begin and end times of the time interval. The timestamps\n" +
                                                                  "are in the coordinate reference system specified in `trs`. By default\n" +
                                                                  "this is the Gregorian calendar.",
                                                    Type = "array",
                                                    MinItems = 2,
                                                    MaxItems = 2,
                                                    Items = new OpenApiSchema
                                                    {
                                                        Type = "string",
                                                        Format = "date-time",
                                                        Nullable = true,
                                                        Example = new OpenApiArray
                                                        {
                                                            new OpenApiString("2011-11-11T12:22:11Z"),
                                                            new OpenApiNull()
                                                        }
                                                    }
                                                }
                                            },
                                            ["trs"] = new()
                                            {
                                                Description = "Coordinate reference system of the coordinates in the temporal extent\n" +
                                                              "(property `interval`). The default reference system is the Gregorian calendar.\n" +
                                                              "In the Core this is the only supported temporal reference system.\n" +
                                                              "Extensions may support additional temporal reference systems and add\n" +
                                                              "additional enum values.",
                                                Type = "string",
                                                Default = new OpenApiString("http://www.opengis.net/def/uom/ISO-8601/0/Gregorian")
                                            }
                                        }
                                    }
                                }
                            },
                            ["itemType"] = new()
                            {
                                Type = "string",
                                Description = "indicator about the type of the items in the collection (the default value is 'feature')",
                                Default = new OpenApiString("feature")
                            },
                            ["crs"] = new()
                            {
                                Description = "the list of coordinate reference systems supported by the service",
                                Type = "array",
                                Items = new OpenApiSchema { Type = "string" },
                                Default = new OpenApiString("http://www.opengis.net/def/crs/OGC/1.3/CRS84")
                            }
                        },
                        Required = new HashSet<string> { "id", "links" }
                    },
                    ["ProblemDetails"] = new()
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["type"] = new()
                            {
                                Type = "string",
                                Nullable = true
                            },
                            ["title"] = new()
                            {
                                Type = "string",
                                Nullable = true
                            },
                            ["status"] = new()
                            {
                                Type = "integer",
                                Format = "int32",
                                Nullable = true
                            },
                            ["detail"] = new()
                            {
                                Type = "string",
                                Nullable = true
                            },
                            ["instance"] = new()
                            {
                                Type = "string",
                                Nullable = true
                            }
                        }
                    },
                    ["Tileset"] = new()
                    {
                        Type = "object",
                        Properties = new Dictionary<string, OpenApiSchema>
                        {
                            ["title"] = new() { Type = "string" },
                            ["tileMatrixSetURI"] = new() { Type = "string" },
                            ["crs"] = new() { Type = "string" },
                            ["dataType"] = new() { Type = "string", Enum = new List<IOpenApiAny> { new OpenApiString("vector") } },
                            ["links"] = new()
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    Reference = new OpenApiReference { Id = "Link", Type = ReferenceType.Schema }
                                }
                            },
                            ["tileMatrixSetLimits"] = new()
                            {
                                Type = "array",
                                Items = new OpenApiSchema
                                {
                                    MinItems = 1,
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        ["tileMatrix"] = new()
                                        {
                                            Type = "integer",
                                            Format = "int32"
                                        },
                                        ["minTileRow"] = new()
                                        {
                                            Type = "integer",
                                            Format = "int32"
                                        },
                                        ["maxTileRow"] = new()
                                        {
                                            Type = "integer",
                                            Format = "int32"
                                        },
                                        ["minTileCol"] = new()
                                        {
                                            Type = "integer",
                                            Format = "int32"
                                        },
                                        ["maxTileCol"] = new()
                                        {
                                            Type = "integer",
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
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Tags = new List<OpenApiTag>
                        {
                            new() { Name = collection.Title }
                        },
                        Summary = "Feature collection metadata",
                        Description = collection.Description,
                        Responses = new OpenApiResponses
                        {
                            ["200"] = new()
                            {
                                Description = "Success",
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new()
                                    {
                                        Schema = new OpenApiSchema
                                        {
                                            Reference = new OpenApiReference { Id = "Collection", Type = ReferenceType.Schema }
                                        }
                                    }
                                }
                            },
                            ["404"] = new()
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
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            Tags = new List<OpenApiTag>
                            {
                                new() { Name = collection.Title }
                            },
                            Summary = "Provides a list of available tilesets for a resource",
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new()
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/json"] = new()
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "array",
                                                Items = new OpenApiSchema
                                                {
                                                    Reference = new OpenApiReference { Id = "Tileset", Type = ReferenceType.Schema }
                                                }
                                            }
                                        }
                                    }
                                },
                                ["404"] = new()
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
                    }
                });

                openApiDocument.Paths.Add($"/collections/{collection.Id}/tiles/{{tileMatrix}}/{{tileRow}}/{{tileCol}}", new OpenApiPathItem
                {
                    Operations = new Dictionary<OperationType, OpenApiOperation>
                    {
                        [OperationType.Get] = new()
                        {
                            Tags = new List<OpenApiTag>
                            {
                                new() { Name = collection.Title }
                            },
                            Summary = "Provides a list of available tilesets for a resource",
                            Parameters = new List<OpenApiParameter>
                            {
                                new()
                                {
                                    Name = "tileMatrix",
                                    Description = "Identifier of the tile matrix (representing a zoom level, a.k.a. a scale) listed in the TileMatrixSet definition",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "integer",
                                        Format = "int32"
                                    }
                                },
                                new()
                                {
                                    Name = "tileRow",
                                    Description = "A non-negative integer between 0 and the MatrixHeight - 1. If there is a TileMatrixSetLimits the value is limited between MinTileRow and MaxTileRow",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "integer",
                                        Format = "int32"
                                    }
                                },
                                new()
                                {
                                    Name = "tileCol",
                                    Description = "A non-negative integer between 0 and the MatrixWidth - 1. If there is a TileMatrixSetLimits the value is limited between MinTileCol and MaxTileCol",
                                    In = ParameterLocation.Path,
                                    Required = true,
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "integer",
                                        Format = "int32"
                                    }
                                }
                            },
                            Responses = new OpenApiResponses
                            {
                                ["200"] = new()
                                {
                                    Description = "Success",
                                    Content = new Dictionary<string, OpenApiMediaType>
                                    {
                                        ["application/vnd.mapbox-vector-tile"] = new()
                                        {
                                            Schema = new OpenApiSchema
                                            {
                                                Type = "file"
                                            }
                                        }
                                    }
                                },
                                ["404"] = new()
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
                    }
                });
            }
        }

        if (_apiOptions.UseApiKeyAuthorization)
        {
            openApiDocument.SecurityRequirements = new List<OpenApiSecurityRequirement>
            {
                new()
                {
                    [new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                    }] = Array.Empty<string>()
                }
            };

            openApiDocument.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["ApiKey"] = new()
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Query,
                    Name = "apiKey",
                    Description = "API key"
                }
            };
        }

        foreach (var extension in extensions)
            extension.Apply(openApiDocument, _apiOptions);

        return openApiDocument;
    }

    private OpenApiSchema GetFeatureCollectionSchema(CollectionOptions collectionOptions)
    {
        return new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string> { "type", "features" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Enum = new List<IOpenApiAny> { new OpenApiString("FeatureCollection") } },
                ["features"] = new()
                {
                    Type = "array",
                    Items = GetFeatureSchema(collectionOptions)
                },
                ["links"] = new()
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference { Id = "Link", Type = ReferenceType.Schema }
                    }
                },
                ["timeStamp"] = new()
                {
                    Type = "string",
                    Format = "date-time"
                },
                ["numberMatched"] = new()
                {
                    Type = "integer",
                    Minimum = 0
                },
                ["numberReturned"] = new()
                {
                    Type = "integer",
                    Minimum = 0
                }
            }
        };
    }

    private OpenApiSchema GetFeatureSchema(CollectionOptions collectionOptions)
    {
        var collectionSourceOptions = collectionOptions.Features.Storage;

        return new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string> { "type", "geometry", "properties" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Enum = new List<IOpenApiAny> { new OpenApiString("Feature") } },
                ["geometry"] = GetGeometrySchema(collectionSourceOptions?.GeometryGeoJsonType),
                ["properties"] = new()
                {
                    Type = "object",
                    Nullable = true,
                    Properties = collectionSourceOptions?.Properties?.ToDictionary(key => key, _ => new OpenApiSchema())
                },
                ["id"] = new()
                {
                    OneOf = new List<OpenApiSchema>
                    {
                        new() { Type = "string" },
                        new() { Type = "integer" }
                    }
                },
                ["links"] = new()
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Reference = new OpenApiReference { Id = "Link", Type = ReferenceType.Schema }
                    }
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
                AnyOf = new List<OpenApiSchema>
                {
                    GetPointSchema(),
                    GetMultiPointSchema()
                }
            },
            "LineString" => GetLineStringSchema(),
            "MultiLineString" => new OpenApiSchema
            {
                AnyOf = new List<OpenApiSchema>
                {
                    GetLineStringSchema(),
                    GetMultiLineStringSchema()
                }
            },
            "Polygon" => GetPolygonSchema(),
            "MultiPolygon" => new OpenApiSchema
            {
                AnyOf = new List<OpenApiSchema>
                {
                    GetPolygonSchema(),
                    GetMultiPolygonSchema()
                }
            },
            _ => null
        };
    }

    private static OpenApiSchema GetPointSchema()
    {
        return new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string", Enum = { new OpenApiString("Point") } },
                ["coordinates"] = new()
                {
                    Type = "array",
                    MinItems = 2,
                    Items = new OpenApiSchema { Type = "number" }
                }
            }
        };
    }

    private static OpenApiSchema GetMultiPointSchema()
    {
        return new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string", Enum = { new OpenApiString("MultiPoint") } },
                ["coordinates"] = new()
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "array",
                        MinItems = 2,
                        Items = new OpenApiSchema { Type = "number" }
                    }
                }
            }
        };
    }

    private static OpenApiSchema GetLineStringSchema()
    {
        return new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string", Enum = { new OpenApiString("LineString") } },
                ["coordinates"] = new()
                {
                    Type = "array",
                    MinItems = 2,
                    Items = new OpenApiSchema
                    {
                        Type = "array",
                        MinItems = 2,
                        Items = new OpenApiSchema { Type = "number" }
                    }
                }
            }
        };
    }

    private static OpenApiSchema GetMultiLineStringSchema()
    {
        return new OpenApiSchema
        {
            Type = "object",
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string", Enum = { new OpenApiString("MultiLineString") } },
                ["coordinates"] = new()
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "array",
                        MinItems = 2,
                        Items = new OpenApiSchema
                        {
                            Type = "array",
                            MinItems = 2,
                            Items = new OpenApiSchema { Type = "number" }
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
            Type = "object",
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string", Enum = { new OpenApiString("Polygon") } },
                ["coordinates"] = new()
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "array",
                        MinItems = 4,
                        Items = new OpenApiSchema
                        {
                            Type = "array",
                            MinItems = 2,
                            Items = new OpenApiSchema { Type = "number" }
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
            Type = "object",
            Required = new HashSet<string> { "type", "coordinates" },
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string", Enum = { new OpenApiString("MultiPolygon") } },
                ["coordinates"] = new()
                {
                    Type = "array",
                    Items = new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "array",
                            MinItems = 4,
                            Items = new OpenApiSchema
                            {
                                Type = "array",
                                MinItems = 2,
                                Items = new OpenApiSchema { Type = "number" }
                            }
                        }
                    }
                }
            }
        };
    }

    private Dictionary<OperationType, OpenApiOperation> GetFeatureOperations(CollectionOptions collection)
    {
        var result = new Dictionary<OperationType, OpenApiOperation>
        {
            [OperationType.Get] = new()
            {
                Tags = new List<OpenApiTag>
                {
                    new() { Name = collection.Title }
                },
                Summary = "Fetch feature",
                Description = $"Fetch the feature with id featureId in the feature collection with id {collection.Id}.",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = "string" }
                    },
                    new()
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the response document will be presented in the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uri"
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new()
                    {
                        Description = "Success",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/geo+json"] = new()
                            {
                                Schema = GetFeatureSchema(collection)
                            }
                        }
                    },
                    ["400"] = new()
                    {
                        Description = "Bad Request",
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
                    },
                    ["404"] = new()
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
        };

        if (collection.Features.Storage.AllowReplace)
        {
            result.Add(OperationType.Put, new OpenApiOperation
            {
                Tags = new List<OpenApiTag>
                {
                    new() { Name = collection.Title }
                },
                Summary = "Replace feature",
                Description = "Replace an existing resource",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature to replace",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = "string" }
                    },
                    new()
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the request document will be converted from the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uri"
                        }
                    }
                },
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/geo+json"] = new()
                        {
                            Schema = GetFeatureSchema(collection)
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new()
                    {
                        Description = "Success"
                    },
                    ["400"] = new()
                    {
                        Description = "Bad Request",
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
                    },
                    ["401"] = new()
                    {
                        Description = "Unauthorized",
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
                    },
                    ["404"] = new()
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
            });
        }

        if (collection.Features.Storage.AllowDelete)
        {
            result.Add(OperationType.Delete, new OpenApiOperation
            {
                Tags = new List<OpenApiTag>
                {
                    new() { Name = collection.Title }
                },
                Summary = "Delete feature",
                Description = "Remove a resource from a collection",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature to delete",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = "string" }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new()
                    {
                        Description = "Success"
                    },
                    ["400"] = new()
                    {
                        Description = "Bad Request",
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
                    },
                    ["401"] = new()
                    {
                        Description = "Unauthorized",
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
                    },
                    ["404"] = new()
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
            });
        }

        if (collection.Features.Storage.AllowUpdate)
        {
            result.Add(OperationType.Patch, new OpenApiOperation
            {
                Tags = new List<OpenApiTag>
                {
                    new() { Name = collection.Title }
                },
                Summary = "Update feature",
                Description = "Modify an existing resource",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "featureId",
                        Description = "Identifier of a feature to update",
                        Required = true,
                        In = ParameterLocation.Path,
                        Schema = new OpenApiSchema { Type = "string" }
                    },
                    new()
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the request document will be converted from the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uri"
                        }
                    }
                },
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/geo+json"] = new()
                        {
                            Schema = GetFeatureSchema(collection)
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new()
                    {
                        Description = "Success"
                    },
                    ["400"] = new()
                    {
                        Description = "Bad Request",
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
                    },
                    ["401"] = new()
                    {
                        Description = "Unauthorized",
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
                    },
                    ["404"] = new()
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
            });
        }

        return result;
    }

    private Dictionary<OperationType, OpenApiOperation> GetFeatureCollectionOperations(CollectionOptions collection)
    {
        var result = new Dictionary<OperationType, OpenApiOperation>
        {
            [OperationType.Get] = new()
            {
                Tags = new List<OpenApiTag>
                {
                    new() { Name = collection.Title }
                },
                Summary = "Fetch features",
                Description = $"Fetch features of the feature collection with id {collection.Id}.\n" +
                              "Every feature in a dataset belongs to a collection. A dataset may consist of multiple feature collections. A feature collection is often a collection of features of a similar type, based on a common schema.",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "limit",
                        Description = "Limits the number of items that are presented in the response document",
                        In = ParameterLocation.Query,
                        Style = ParameterStyle.Form,
                        Explode = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int32",
                            Default = new OpenApiInteger(10),
                            Minimum = 1,
                            Maximum = 10000
                        }
                    },
                    new()
                    {
                        Name = "offset",
                        Description = "Offset for requesting objects. The resulting response will contain a link for the next features page",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "integer",
                            Format = "int32",
                            Default = new OpenApiInteger(0)
                        }
                    },
                    new()
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
                            Type = "array",
                            MinItems = 4,
                            MaxItems = 6,
                            Items = new OpenApiSchema
                            {
                                Type = "number"
                            }
                        }
                    },
                    new()
                    {
                        Name = "bbox-crs",
                        Description = "Parameter may be used to assert the CRS used for the coordinate values of the bbox parameter. " +
                                      "If the bbox-crs parameter is not specified then the coordinate values of the bbox parameter is the default CRS that is http://www.opengis.net/def/crs/OGC/1.3/CRS84",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uri"
                        }
                    },
                    new()
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
                            Type = "string"
                        }
                    },
                    new()
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the response document will be presented in the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uri"
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["200"] = new()
                    {
                        Description = "Success",
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/geo+json"] = new()
                            {
                                Schema = GetFeatureCollectionSchema(collection)
                            }
                        }
                    },
                    ["400"] = new()
                    {
                        Description = "Bad Request",
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
                    },
                    ["404"] = new()
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
        };

        if (collection.Features.Storage.AllowCreate)
        {
            result.Add(OperationType.Post, new OpenApiOperation
            {
                Tags = new List<OpenApiTag>
                {
                    new() { Name = collection.Title }
                },
                Summary = "Create feature",
                Description = "Add a new resource to a collection",
                Parameters = new List<OpenApiParameter>
                {
                    new()
                    {
                        Name = "crs",
                        Description = "The coordinates of all geometry-valued properties in the request document will be converted from the requested CRS",
                        In = ParameterLocation.Query,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "uri"
                        }
                    }
                },
                RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/geo+json"] = new()
                        {
                            Schema = GetFeatureSchema(collection)
                        }
                    }
                },
                Responses = new OpenApiResponses
                {
                    ["201"] = new()
                    {
                        Description = "Success",
                        Headers = new Dictionary<string, OpenApiHeader>
                        {
                            ["Location"] = new()
                        }
                    },
                    ["400"] = new()
                    {
                        Description = "Bad Request",
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
                    },
                    ["401"] = new()
                    {
                        Description = "Unauthorized",
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
                    },
                    ["404"] = new()
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
            });
        }

        return result;
    }
}