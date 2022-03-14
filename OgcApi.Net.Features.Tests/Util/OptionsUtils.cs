using OgcApi.Net.Features.Options;
using Microsoft.Extensions.Configuration;
using OgcApi.Net.Features.Options.SqlOptions;

namespace OgcApi.Net.Features.Tests.Util
{
    public static class OptionsUtils
    {
        public static OgcApiOptions GetOptionsFromJson()
        {
            var config = new ConfigurationBuilder().AddJsonFile("//Util//appsettings_test.json").Build();
            var options = new OgcApiOptions();
            config.GetSection("FeaturesOptions").Bind(options);
            return options;
        }

        public static OgcApiOptions GetOptionsFromCode()
        {
            return new()
            {
                LandingPage = new()
                {
                    Title = "API Title",
                    Description = "API Description",
                    ContactName = "API Contacts",
                    ContactUrl = new("https://example.com/"),
                    ApiDocumentPage = new("https://api.com/index.html"),
                    ApiDescriptionPage = new("https://api.com/swagger.json"),
                    Links = new()
                    {
                        new() { Href = new("https://api.com/landing_page/link1.html") },
                        new() { Href = new("https://api.com/landing_page/link2.html") }
                    }
                },
                Conformance = new()
                {
                    ConformsTo = new()
                    {
                        new("https://api.com/conform1.html"),
                        new("https://api.com/conform2.html")
                    }
                },
                UseApiKeyAuthorization = true,
                Collections = new()
                {
                    Links = new()
                    {
                        new() { Href = new("https://api.com/collections/link1.html") },
                        new() { Href = new("https://api.com/collections/link2.html") }
                    },
                    Items = new()
                    {
                        new()
                        {
                            Id = "Collection1",
                            Title="Collection title 1",
                            Description = "Collection description 1",
                            Features = new()
                            {
                                Crs = new()
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                },
                                StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                Storage = new SqlCollectionSourceOptions()
                                {
                                    Type = "PostGIS",
                                    ConnectionString = "Host=127.0.0.1;User Id=user;Password=user;Database=pgdb;Port=5432",
                                    Schema = "api",
                                    Table = "collection1",
                                    GeometryColumn = "geom",
                                    GeometrySrid = 3857,
                                    GeometryDataType = "geometry",
                                    GeometryGeoJsonType = "MultiPolygon",
                                    IdentifierColumn = "id",
                                    Properties = new()
                                    {
                                        "prop1",
                                        "prop2"
                                    },
                                    AllowCreate = true,
                                    AllowReplace = true,
                                    AllowUpdate = true,
                                    ApiKeyPredicateForGet = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                    ApiKeyPredicateForCreate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                    ApiKeyPredicateForUpdate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)"
                                }
                            }
                        },
                        new()
                        {
                            Id = "Collection2",
                            Title = "Collection title 2",
                            Description = "Collection description 2",
                            Features = new()
                            {
                                Crs = new()
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                },
                                StorageCrs = new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                Storage = new SqlCollectionSourceOptions()
                                {
                                    Type = "SqlServer",
                                    ConnectionString = "data source=127.0.0.1,1433;user id=user;password=user;initial catalog=dbo;Persist Security Info=true",
                                    Schema = "dbo",
                                    Table = "Collection2",
                                    GeometryColumn = "Geom",
                                    GeometrySrid = 3857,
                                    GeometryDataType = "Geometry",
                                    GeometryGeoJsonType = "MultiPolygon",
                                    IdentifierColumn = "Id",
                                    Properties = new()
                                    {
                                        "prop1",
                                        "prop2"
                                    },
                                    AllowCreate = true,
                                    AllowReplace = true,
                                    AllowUpdate = true,
                                    ApiKeyPredicateForGet = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                    ApiKeyPredicateForCreate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)",
                                    ApiKeyPredicateForUpdate = "EXISTS(SELECT ... WHERE @ApiKey = apiKey)"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
