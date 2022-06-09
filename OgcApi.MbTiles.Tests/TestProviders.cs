using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Net.MbTiles;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Tiles;
using System;
using System.Collections.Generic;
using System.IO;

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

        private static bool TestMBTilesAccessDelegate(string collectionId, int tileMatrix, int tileRow, int tileCol, string apiKey)
        {
            if ((apiKey ?? "").Equals("qwerty") && (collectionId ?? "").Equals("data") && ((tileMatrix >= 0) && (tileMatrix <= 7)))
            {
                switch (tileMatrix)
                {
                    case 0: return ((tileRow == 0) && (tileCol == 0));
                    case 1: return ((tileRow == 0) && (tileCol == 1));
                    case 2: return ((tileRow == 1) && (tileCol == 2));
                    case 3: return ((tileRow == 2) && (tileCol == 5));
                    case 4: return ((tileRow == 5) && (tileCol == 10));
                    case 5: return ((tileRow == 10) && (tileCol == 20));
                    case 6: return ((tileRow == 20) && (tileCol == 40));
                    case 7: return ((tileRow == 40) && (tileCol == 81 || tileCol == 82));
                }
            }
            return false;
        }

        private static OgcApiOptions GetOptionsWithAccessDelegate()
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
                                    TileAccessDelegate = TestMBTilesAccessDelegate
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

        public static MbTilesProvider GetProviderWithAccessDelegate()
        {
            return new MbTilesProvider(new NullLogger<MbTilesProvider>(),
                Mock.Of<IOptionsMonitor<OgcApiOptions>>(_ => _.CurrentValue == GetOptionsWithAccessDelegate()));
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
