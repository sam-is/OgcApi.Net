using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OgcApi.Net;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Converters;
using OgcApi.Net.Options.Features;
using OgcApi.Net.PostGis;
using OgcApi.Net.Resources;
using OgcApi.Net.SqlServer;
using System;
using System.Text.Json;

namespace OgcApi.Options.Tests.Utils;

public static class OptionsUtils
{
    private static ServiceProvider Provider { get; set; }

    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        Converters = { new FeaturesSourceOptionsConverter() }
    };

    public static OgcApiOptions GetOptionsFromJsonConfig()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddLogging();
        serviceCollection.AddOgcApiPostGisProvider();
        serviceCollection.AddOgcApiSqlServerProvider();
        serviceCollection.AddOgcApi("ogcsettings.json");

        Provider = serviceCollection.BuildServiceProvider();

        return Provider.GetRequiredService<IOptions<OgcApiOptions>>().Value;
    }

    public static OgcApiOptions GetOptionsFromCode()
    {
        return new OgcApiOptions
        {
            LandingPage = new LandingPageOptions
            {
                Title = "API Title",
                Description = "API Description",
                ContactName = "API Contacts",
                ContactUrl = new Uri("https://example.com/"),
                ApiDocumentPage = new Uri("https://api.com/index.html"),
                ApiDescriptionPage = new Uri("https://api.com/swagger.json"),
                Links =
                [
                    new Link {Href = new Uri("https://api.com/landing_page/link1.html")},
                    new Link {Href = new Uri("https://api.com/landing_page/link2.html")}
                ]
            },
            Conformance = new ConformanceOptions
            {
                ConformsTo =
                [
                    new Uri("https://api.com/conform1.html"),
                    new Uri("https://api.com/conform2.html")
                ]
            },
            UseApiKeyAuthorization = true,
            Collections = new CollectionsOptions
            {
                Links =
                [
                    new Link {Href = new Uri("https://api.com/collections/link1.html")},
                    new Link {Href = new Uri("https://api.com/collections/link2.html")}
                ],
                Items =
                [
                    new CollectionOptions
                    {
                        Id = "Collection1",
                        Title = "Collection title 1",
                        Description = "Collection description 1",
                        Extent = new Extent
                        {
                            Spatial = new SpatialExtent
                            {
                                Bbox = [[1.0, 2.0], [3.0, 4.0]],
                                Crs = new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84")
                            },
                            Temporal = new TemporalExtent
                                {Interval = [[1.0, 2.0], [3.0, 4.0]], Trs = "Trs"}
                        },
                        Links =
                        [
                            new Link {Href = new Uri("https://api.com/collections/collection1/link1.html")},
                            new Link {Href = new Uri("https://api.com/collections/collection1/link2.html")}
                        ],
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "PostGis",
                                ConnectionString = "Host=127.0.0.1;User Id=user;Password=user;Database=pgdb;Port=5432",
                                Schema = "api",
                                Table = "collection1",
                                GeometryColumn = "Geom",
                                GeometrySrid = 3857,
                                GeometryDataType = "geometry",
                                GeometryGeoJsonType = "MultiPolygon",
                                IdentifierColumn = "id",
                                Properties =
                                [
                                    "Prop1",
                                    "Prop2"
                                ],
                                AllowCreate = true,
                                AllowReplace = true,
                                AllowUpdate = true,
                                ApiKeyPredicateForGet = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                ApiKeyPredicateForCreate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                ApiKeyPredicateForUpdate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)"
                            }
                        }
                    },

                    new CollectionOptions
                    {
                        Id = "Collection2",
                        Title = "Collection title 2",
                        Description = "Collection description 2",
                        Links =
                        [
                            new Link {Href = new Uri("https://api.com/collections/collection2/link1.html")},
                            new Link {Href = new Uri("https://api.com/collections/collection2/link2.html")}
                        ],
                        Features = new CollectionFeaturesOptions
                        {
                            Crs =
                            [
                                new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                new Uri("http://www.opengis.net/def/crs/EPSG/0/3857")
                            ],
                            StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new SqlFeaturesSourceOptions
                            {
                                Type = "SqlServer",
                                ConnectionString =
                                    "data source=127.0.0.1,1433;user id=user;password=user;initial catalog=dbo;Persist Security Info=true",
                                Schema = "dbo",
                                Table = "Collection2",
                                GeometryColumn = "Geom",
                                GeometrySrid = 3857,
                                GeometryDataType = "geometry",
                                GeometryGeoJsonType = "MultiPolygon",
                                IdentifierColumn = "Id",
                                Properties =
                                [
                                    "Prop1",
                                    "Prop2"
                                ],
                                AllowCreate = true,
                                AllowReplace = true,
                                AllowUpdate = true,
                                ApiKeyPredicateForGet = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                ApiKeyPredicateForCreate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                ApiKeyPredicateForUpdate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)"
                            }
                        }
                    }
                ]
            }
        };
    }
}