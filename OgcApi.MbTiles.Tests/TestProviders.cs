using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Net.MbTiles;
using OgcApi.Net.Options;
using System;
using System.Collections.Generic;

namespace OgcApi.MbTiles.Tests
{
    public static class TestProviders
    {
        private static TileSourcesOptions GetDefaultOptions()
        {
            return new TileSourcesOptions()
            {
                Sources = new List<TileSourceOptions>()
                {
                    new MbTilesSourceOptions()
                    {
                        Id = "data",
                        TileMatrixSet = new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                        Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                        ConnectionString = "Data Source=Data\\data.mbtiles"
                    }
                }
            };
        }

        private static TileSourcesOptions GetOptionsWithUnknownDataFile()
        {
            return new TileSourcesOptions()
            {
                Sources = new List<TileSourceOptions>()
                {
                    new MbTilesSourceOptions()
                    {
                        Id = "data",
                        TileMatrixSet = new Uri("http://www.opengis.net/def/tilematrixset/OGC/1.0/WorldMercatorWGS84Quad"),
                        Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3395"),
                        ConnectionString = "Data Source=Data\\test.mbtiles"
                    }
                }
            };
        }

        public static MbTilesProvider GetDefaultProvider()
        {
            TileSourcesOptions options = GetDefaultOptions();
            var optionsMonitor = Mock.Of<IOptionsMonitor<TileSourcesOptions>>(mock => mock.CurrentValue == options);
            return new MbTilesProvider(optionsMonitor, new NullLogger<MbTilesProvider>());
        }

        public static MbTilesProvider GetProviderWithErrors()
        {
            TileSourcesOptions options = GetOptionsWithUnknownDataFile();
            var optionsMonitor = Mock.Of<IOptionsMonitor<TileSourcesOptions>>(mock => mock.CurrentValue == options);
            return new MbTilesProvider(optionsMonitor, new NullLogger<MbTilesProvider>());
        }
    }
}
