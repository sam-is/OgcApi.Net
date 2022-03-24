using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Options.Features;
using OgcApi.Net.Features.PostGis;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Converters;
using OgcApi.Net.Options.Features;
using OgcApi.Net.Resources;
using OgcApi.Net.SqlServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace OgcApi.Net.Features.Tests.Utils
{
    public static class OptionsUtils
    {
        private static ServiceProvider Provider { get; set; }

        public static IFeaturesProvider GetDataProvider(string dbType)
        {
            return Net.Utils.GetFeaturesProvider(Provider, dbType);
        }
        public static void SetupServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            serviceCollection.AddOgcApiPostGisProvider();
            serviceCollection.AddOgcApiSqlServerProvider();
            Provider = serviceCollection.BuildServiceProvider();
        }

        public static OgcApiOptions GetOptionsFromJson()
        {
            var jsonReadOnlySpan = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ogcsettings.json"));
            var reader = new Utf8JsonReader(jsonReadOnlySpan);
            var converter = new OgcApiOptionsConverter(Provider);
            var options = converter.Read(ref reader, typeof(OgcApiOptions), new JsonSerializerOptions());
            return options;
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
                    Links = new List<Link>
                    {
                        new() { Href = new Uri("https://api.com/landing_page/link1.html") },
                        new() { Href = new Uri("https://api.com/landing_page/link2.html") }
                    }
                },
                Conformance = new ConformanceOptions
                {
                    ConformsTo = new List<Uri>
                    {
                        new("https://api.com/conform1.html"),
                        new("https://api.com/conform2.html")
                    }
                },
                UseApiKeyAuthorization = true,
                Collections = new CollectionsOptions
                {
                    Links = new List<Link>
                    {
                        new() { Href = new Uri("https://api.com/collections/link1.html") },
                        new() { Href = new Uri("https://api.com/collections/link2.html") }
                    },
                    Items = new List<CollectionOptions>
                    {
                        new()
                        {
                            Id = "Collection1",
                            Title = "Collection title 1",
                            Description = "Collection description 1",
                            Extent = new Extent
                            {
                                Spatial = new SpatialExtent { Bbox = new[] { new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 } }, Crs = new Uri("http://www.opengis.net/def/crs/OGC/1.3/CRS84") },
                                Temporal = new TemporalExtent { Interval = new[] { new[] { 1.0, 2.0 }, new[] { 3.0, 4.0 } }, Trs = "Trs" }
                            },
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                },
                                StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                Storage = new SqlFeaturesSourceOptions
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
                                    Properties = new List<string>
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
                            Features = new CollectionFeaturesOptions
                            {
                                Crs = new List<Uri>
                                {
                                    new("http://www.opengis.net/def/crs/OGC/1.3/CRS84"),
                                    new("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                },
                                StorageCrs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                                Storage = new SqlFeaturesSourceOptions
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
                                    Properties = new List<string>
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

        public static OgcApiOptions GetOptionsFromConfiguration()
        {
            var conf = new ConfigureOgcApiOptions(Provider.GetRequiredService<IServiceScopeFactory>());
            var options = new OgcApiOptions();
            conf.Configure(options);
            return options;
        }

        public static string SerializeOgcApiOptions(OgcApiOptions options)
        {
            var ms = new MemoryStream();
            var writer = new Utf8JsonWriter(ms);
            var converter = new OgcApiOptionsConverter(Provider);
            converter.Write(writer, options, new JsonSerializerOptions());
            writer.Flush();
            ms.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string SerializeLandingPageOptions(LandingPageOptions options)
        {
            var ms = new MemoryStream();
            var writer = new Utf8JsonWriter(ms);
            var converter = new OgcApiOptionsConverter(Provider);
            converter.WriteLandingPage(writer, options);
            writer.Flush();
            ms.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string SerializeCollectionsOptions(CollectionsOptions options)
        {
            var ms = new MemoryStream();
            var writer = new Utf8JsonWriter(ms);
            var converter = new OgcApiOptionsConverter(Provider);
            converter.WriteCollections(writer, options, new());
            writer.Flush();
            ms.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }

    }
}
