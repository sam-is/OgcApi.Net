using Microsoft.Extensions.Logging.Abstractions;
using OgcApi.Net.MbTiles;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using Moq;

namespace OgcApi.MbTiles.Tests
{
    public static class TestProviders
    {
        private static OgcApiOptions GetDefaultOptions()
        {
            return new OgcApiOptions
            {
                Collections = new CollectionsOptions
                {
                    Items = new List<CollectionOptions>
                    {
                        new()
                        {
                            Title = "data",
                            Id = "data",
                            Tiles = new CollectionTilesOptions
                            {
                                TileMatrixSet =
                                    new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                                Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                                Storage = new MbTilesSourceOptions()
                                {
                                    Type = "MbTiles",
                                    FileName = Path.Combine("Data", "data.mbtiles")
                                }
                            }
                        }
                    }
                }
            };
        }

        private static OgcApiOptions GetOptionsWithMinMaxZoom()
        {
            return new OgcApiOptions
            {
                Collections = new CollectionsOptions
                {
                    Items = new List<CollectionOptions>
                    {
                        new()
                        {
                            Title = "data",
                            Id = "data",
                            Tiles = new CollectionTilesOptions
                            {
                                TileMatrixSet =
                                    new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                                Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                                Storage = new MbTilesSourceOptions()
                                {
                                    Type = "MbTiles",
                                    FileName = Path.Combine("Data", "data.mbtiles"),
                                    MinZoom = 5,
                                    MaxZoom = 9,
                                }
                            }
                        }
                    }
                }
            };
        }

        private static OgcApiOptions GetOptionsWithUnknownDataFile()
        {
            return new OgcApiOptions
            {
                Collections = new CollectionsOptions
                {
                    Items = new List<CollectionOptions>
                    {
                        new()
                        {
                            Title = "data",
                            Id = "data",
                            Tiles = new CollectionTilesOptions
                            {
                                TileMatrixSet =
                                    new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                                Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                                Storage = new MbTilesSourceOptions()
                                {
                                    Type = "MbTiles",
                                    FileName = "Data Source=" + Path.Combine("Data", "test.mbtiles")
                                }
                            }
                        }
                    }
                }
            };
        }

        public static MbTilesProvider GetDefaultProvider()
        {
            return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetDefaultOptions()));
        }

        public static MbTilesProvider GetProviderWithErrors()
        {
            return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetOptionsWithUnknownDataFile()));
        }

        public static MbTilesProvider GetProviderWithMinMaxZoom()
        {
            return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetOptionsWithMinMaxZoom()));
        }
    }
}
