using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.PostGis;
using OgcApi.Net.Features.SqlServer;
using OgcApi.Net.Features;

namespace OgcApi.Net.Features.Tests.Util
{
    public static class OptionsUtils
    {
        private static ServiceProvider Provider { get; set; }
        public static IDataProvider GetDataProvider(string dbType)
        {
            return Utils.GetDataProvider(Provider, dbType);
        }
        private static void SetupServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddOgcApiPostGisProvider();
            serviceCollection.AddOgcApiSqlServerProvider();           
            Provider = serviceCollection.BuildServiceProvider();
            
        }

        public static OgcApiOptions GetOptionsFromJson()
        {
            SetupServiceCollection();

            var jsonReadOnlySpan = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Util", "appsettings_test.json"));
            var reader = new Utf8JsonReader(jsonReadOnlySpan);
            var converter = new OgcApiOptionsConverter(Provider);
            var options = converter.Read(ref reader, typeof(OgcApiOptions), new());
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
                            Title = "Collection title 1",
                            Description = "Collection description 1",
                            Extent = new()
                            {
                                Spatial = new() { Bbox = new[] { new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 } }, Crs = new("http://www.opengis.net/def/crs/OGC/1.3/CRS84") },
                                Temporal = new() { Interval = new[] { new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 } }, Trs = "Trs" }
                            },
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

        public static string SerializeOptions(OgcApiOptions options)
        {
            SetupServiceCollection();

            var ms = new MemoryStream();
            var writer = new Utf8JsonWriter(ms);
            var converter = new OgcApiOptionsConverter(Provider);
            converter.Write(writer, options, new());
            writer.Flush();
            ms.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
