using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NetTopologySuite.Features;
using OgcApi.Net;
using OgcApi.Net.Controllers;
using OgcApi.Net.MbTiles;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Tiles;
using System;
using System.IO;

namespace OgcApi.MbTiles.Tests;

public static class TestProviders
{
    private static OgcApiOptions GetDefaultOptions()
    {
        return new OgcApiOptions
        {
            Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Title = "data",
                        Id = "data",
                        Tiles = new CollectionTilesOptions
                        {
                            TileMatrixSet =
                                new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                            Storage = new MbTilesSourceOptions
                            {
                                Type = "MbTiles",
                                FileName = Path.Combine("Data", "data.mbtiles"),
                                TimestampFiles =
                                {
                                    new TimestampFile
                                    {
                                        DateTime = new DateTime(2018, 2, 12),
                                        FileName = Path.Combine("Data", "data_12-02-2018.mbtiles")
                                    }
                                }
                            }
                        }
                    }
                ]
            }
        };
    }

    private static OgcApiOptions GetOptionsWithMinMaxZoom()
    {
        return new OgcApiOptions
        {
            Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Title = "data",
                        Id = "data",
                        Tiles = new CollectionTilesOptions
                        {
                            TileMatrixSet =
                                new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                            Storage = new MbTilesSourceOptions
                            {
                                Type = "MbTiles",
                                FileName = Path.Combine("Data", "data.mbtiles"),
                                MinZoom = 5,
                                MaxZoom = 9,
                            }
                        }
                    }
                ]
            }
        };
    }

    private static bool TestMbTilesAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey)
    {
        if (!(apiKey ?? "").Equals("qwerty") || !(collectionId ?? "").Equals("data")) return false;
        return tileMatrix switch
        {
            0 => tileRow == 0 && tileCol == 0,
            1 => tileRow == 0 && tileCol == 1,
            2 => tileRow == 1 && tileCol == 2,
            3 => tileRow == 2 && tileCol == 5,
            4 => tileRow == 5 && tileCol == 10,
            5 => tileRow == 10 && tileCol == 20,
            6 => tileRow == 20 && tileCol == 40,
            7 => tileRow == 40 && tileCol is 81 or 82,
            _ => false
        };
    }

    private static OgcApiOptions GetOptionsWithAccessDelegate()
    {
        return new OgcApiOptions
        {
            LandingPage = new LandingPageOptions
            {
                ApiDocumentPage = new Uri("https://api.com/index.html"),
                ApiDescriptionPage = new Uri("https://api.com/swagger.json")
            },
            Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Title = "data",
                        Id = "data",
                        Tiles = new CollectionTilesOptions
                        {
                            TileMatrixSet =
                                new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                            Storage = new MbTilesSourceOptions
                            {
                                Type = "MbTiles",
                                FileName = Path.Combine("Data", "data.mbtiles"),
                                TileAccessDelegate = TestMbTilesAccessDelegate
                            }
                        }
                    }
                ]
            }
        };
    }

    private static OgcApiOptions GetOptionsWithUnknownDataFile()
    {
        return new OgcApiOptions
        {
            Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Title = "data",
                        Id = "data",
                        Tiles = new CollectionTilesOptions
                        {
                            TileMatrixSet =
                                new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                            Storage = new MbTilesSourceOptions
                            {
                                Type = "MbTiles",
                                FileName = "Data Source=" + Path.Combine("Data", "test.mbtiles")
                            }
                        }
                    }
                ]
            }
        };
    }

    public static MbTilesProvider GetDefaultProvider()
    {
        return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetDefaultOptions()));
    }

    public static MbTilesProvider GetProviderWithAccessDelegate()
    {
        return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetOptionsWithAccessDelegate()));
    }

    public static MbTilesProvider GetProviderWithErrors()
    {
        return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetOptionsWithUnknownDataFile()));
    }

    public static MbTilesProvider GetProviderWithMinMaxZoom()
    {
        return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
            Mock.Of<IOptionsMonitor<OgcApiOptions>>(monitor => monitor.CurrentValue == GetOptionsWithMinMaxZoom()));
    }

    public static CollectionsController GetControllerWithAccessDelegate()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOgcApi(options =>
        {
            options.LandingPage = new LandingPageOptions
            {
                ApiDocumentPage = new Uri("https://api.com/index.html"),
                ApiDescriptionPage = new Uri("https://api.com/swagger.json")
            };
            options.Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Title = "data",
                        Id = "data",
                        Tiles = new CollectionTilesOptions
                        {
                            TileMatrixSet =
                                new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                            Storage = new MbTilesSourceOptions
                            {
                                Type = "MbTiles",
                                FileName = Path.Combine("Data", "data.mbtiles"),
                                TileAccessDelegate = TestMbTilesAccessDelegate
                            }
                        }
                    }
                ]
            };
        });
        serviceCollection.AddLogging();
        serviceCollection.AddOgcApiMbTilesProvider();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OgcApiOptions>>();

        var controller = new CollectionsController(options,
            serviceProvider,
            serviceProvider.GetService<ILoggerFactory>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
    public static CollectionsController GetControllerWithoutAccessDelegate()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOgcApi(options =>
        {
            options.LandingPage = new LandingPageOptions
            {
                ApiDocumentPage = new Uri("https://api.com/index.html"),
                ApiDescriptionPage = new Uri("https://api.com/swagger.json")
            };
            options.Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Title = "data",
                        Id = "data",
                        Tiles = new CollectionTilesOptions
                        {
                            TileMatrixSet =
                                new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                            Storage = new MbTilesSourceOptions
                            {
                                Type = "MbTiles",
                                FileName = Path.Combine("Data", "data.mbtiles")
                            }
                        }
                    }
                ]
            };
        });
        serviceCollection.AddLogging();
        serviceCollection.AddOgcApiMbTilesProvider();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OgcApiOptions>>();

        var controller = new CollectionsController(options,
            serviceProvider,
            serviceProvider.GetService<ILoggerFactory>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }

    private static bool TestMbTilesFeatureAccessDelegate(string collectionId, IFeature feature, string apiKey) => (collectionId ?? "") switch
    {
        "featureAccessData" => apiKey == "admin" ||
            apiKey == "value" && feature.Attributes.Exists("value") &&
            (feature.Attributes["value"] is long and > 1200 ||
             feature.Attributes["value"] is > 100.0) ||
            feature.Attributes.Exists("roleAccess") && feature.Attributes["roleAccess"].ToString() == apiKey,
        _ => true,
    };

    public static CollectionsController GetControllerWithFeatureAccessDelegate()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOgcApi(options =>
        {
            options.LandingPage = new LandingPageOptions
            {
                ApiDocumentPage = new Uri("https://api.com/index.html"),
                ApiDescriptionPage = new Uri("https://api.com/swagger.json")
            };
            options.Collections = new CollectionsOptions
            {
                Items =
                [
                    new CollectionOptions
                    {
                        Title = "featureAccessData",
                        Id = "featureAccessData",
                        Tiles = new CollectionTilesOptions
                        {
                            TileMatrixSet =
                                new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                            Storage = new MbTilesSourceOptions
                            {
                                Type = "MbTiles",
                                FileName = Path.Combine("Data", "featureAccessData.mbtiles"),
                                FeatureAccessDelegate = TestMbTilesFeatureAccessDelegate
                            }
                        }
                    }
                ]
            };
        });
        serviceCollection.AddLogging();
        serviceCollection.AddOgcApiMbTilesProvider();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<OgcApiOptions>>();

        var controller = new CollectionsController(options,
            serviceProvider,
            serviceProvider.GetService<ILoggerFactory>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        return controller;
    }
}