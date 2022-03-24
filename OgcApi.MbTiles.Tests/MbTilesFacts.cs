using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using Moq;
using OgcApi.Net.MbTiles;
using OgcApi.Net.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace OgcApi.MbTiles.Tests
{
    public class MbTilesFacts
    {
        [Fact]
        public void ConstructorValid()
        {
            Assert.NotNull(TestProviders.GetDefaultProvider());
        }

        [Fact]
        public void ValidateWrongOptions()
        {
            var options = new CollectionsOptions
            {
                Items = new List<CollectionOptions>
                {
                    new()
                    {
                        Title = "test",
                        Id =  "test",
                        Tiles = new()
                        {
                            Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                            Storage = new MbTilesSourceOptions()
                            {
                                Type = "MbTiles"
                            }
                        }
                    }
                }
            };
            Assert.Throws<OptionsValidationException>(() =>
                CollectionsOptionsValidator.Validate(options));
        }

        [Fact]
        public void ConstructorNullOptions()
        {
            Assert.Throws<OptionsValidationException>(() =>
                CollectionsOptionsValidator.Validate(null));
        }

        [Fact]
        public async void GetTile()
        {
            var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 162, 82);
            Assert.NotNull(tile);
        }

        [Fact]
        public void GetTileUnknownCollection()
        {
            Assert.ThrowsAsync<ArgumentException>(() => TestProviders.GetDefaultProvider().GetTileAsync("test", 8, 162, 82));
        }

        [Fact]
        public async void GetTileIncorrectZoomLevel()
        {
            var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 15, 162, 82);
            Assert.Null(tile);
        }

        [Fact]
        public async void GetTileIncorrectTileRow()
        {
            var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 162, 90);
            Assert.Null(tile);
        }

        [Fact]
        public void GetTileIncorrectTileCol()
        {
            var tile = TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 170, 82);
            Assert.Null(tile.Result);
        }

        [Fact]
        public void GetTileFileNotExists()
        {
            Assert.ThrowsAsync<SqliteException>(() => TestProviders.GetProviderWithErrors().GetTileAsync("data", 8, 162, 82));
        }

        [Fact]
        public void GetLimits()
        {
            var limits = TestProviders.GetDefaultProvider().GetLimits("data");
            Assert.Equal(11, limits.Count);
            var item = limits[0];
            Assert.True(item.TileMatrix == 0 && item.MinTileCol == 0 && item.MaxTileCol == 0 && item.MinTileRow == 0 && item.MaxTileRow == 0);
            item = limits[1];
            Assert.True(item.TileMatrix == 1 && item.MinTileCol == 1 && item.MaxTileCol == 1 && item.MinTileRow == 0 && item.MaxTileRow == 0);
            item = limits[2];
            Assert.True(item.TileMatrix == 2 && item.MinTileCol == 2 && item.MaxTileCol == 2 && item.MinTileRow == 1 && item.MaxTileRow == 1);
            item = limits[3];
            Assert.True(item.TileMatrix == 3 && item.MinTileCol == 5 && item.MaxTileCol == 5 && item.MinTileRow == 2 && item.MaxTileRow == 2);
            item = limits[4];
            Assert.True(item.TileMatrix == 4 && item.MinTileCol == 10 && item.MaxTileCol == 10 && item.MinTileRow == 5 && item.MaxTileRow == 5);
            item = limits[5];
            Assert.True(item.TileMatrix == 5 && item.MinTileCol == 20 && item.MaxTileCol == 20 && item.MinTileRow == 10 && item.MaxTileRow == 10);
            item = limits[6];
            Assert.True(item.TileMatrix == 6 && item.MinTileCol == 40 && item.MaxTileCol == 41 && item.MinTileRow == 20 && item.MaxTileRow == 21);
            item = limits[7];
            Assert.True(item.TileMatrix == 7 && item.MinTileCol == 80 && item.MaxTileCol == 82 && item.MinTileRow == 40 && item.MaxTileRow == 42);
            item = limits[8];
            Assert.True(item.TileMatrix == 8 && item.MinTileCol == 161 && item.MaxTileCol == 165 && item.MinTileRow == 81 && item.MaxTileRow == 84);
            item = limits[9];
            Assert.True(item.TileMatrix == 9 && item.MinTileCol == 323 && item.MaxTileCol == 330 && item.MinTileRow == 162 && item.MaxTileRow == 169);
            item = limits[10];
            Assert.True(item.TileMatrix == 10 && item.MinTileCol == 646 && item.MaxTileCol == 661 && item.MinTileRow == 325 && item.MaxTileRow == 338);
        }

        [Fact]
        public void GetLimitsUnknownCollection()
        {
            Assert.Throws<ArgumentException>(() => TestProviders.GetDefaultProvider().GetLimits("test"));
        }

        [Fact]
        public void GetLimitsFileNotExists()
        {
            Assert.Throws<SqliteException>(() => TestProviders.GetProviderWithErrors().GetLimits("data"));
        }

    }
}
