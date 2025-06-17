using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using NetTopologySuite.IO.VectorTiles.Mapbox;
using OgcApi.Net.MbTiles;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Tiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xunit;

namespace OgcApi.MbTiles.Tests;

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
            Items =
            [
                new CollectionOptions
                {
                    Title = "test",
                    Id = "test",
                    Tiles = new CollectionTilesOptions
                    {
                        Crs = new Uri("http://www.opengis.net/def/crs/EPSG/0/3857"),
                        Storage = new MbTilesSourceOptions
                        {
                            Type = "MbTiles"
                        }
                    }
                }
            ]
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
    public async Task GetTile()
    {
        var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 82, 162);
        Assert.NotNull(tile);
    }

    [Fact]
    public async Task GetTileWithDate()
    {
        var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 82, 162, "2018-02-19T19:00:01Z");
        Assert.NotNull(tile);
    }

    [Fact]
    public async Task GetTileWithIncorrectDate()
    {
        var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 82, 162, "2017-02-12T23:00:01Z");
        Assert.Null(tile);
    }

    [Fact]
    public async Task GetTileDirect()
    {
        var tile = await MbTilesProvider.GetTileDirectAsync(Path.Combine("Data", "data.mbtiles"), 8, 82, 162);
        Assert.NotNull(tile);
    }

    [Fact]
    public async Task GetTileUnknownCollection()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => TestProviders.GetDefaultProvider().GetTileAsync("test", 8, 162, 82));
    }

    [Fact]
    public async Task GetTileIncorrectZoomLevel()
    {
        var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 15, 162, 82);
        Assert.Null(tile);
    }

    [Fact]
    public async Task GetTileIncorrectTileRow()
    {
        var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 162, 90);
        Assert.Null(tile);
    }

    [Fact]
    public async Task GetTileIncorrectTileCol()
    {
        var tile = await TestProviders.GetDefaultProvider().GetTileAsync("data", 8, 170, 82);
        Assert.Null(tile);
    }

    [Fact]
    public async Task GetTileFileNotExists()
    {
        await Assert.ThrowsAsync<SqliteException>(() => TestProviders.GetProviderWithErrors().GetTileAsync("data", 8, 162, 82));
    }

    [Fact]
    public async Task GetTileAccessViolationApiKeyNull()
    {
        var result = await TestProviders.GetControllerWithAccessDelegate().GetTile("data", 7, 40, 81);
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetTileAccessViolationApiKeyIncorrect()
    {
        var result = await TestProviders.GetControllerWithAccessDelegate().GetTile("data", 7, 40, 81, "12345");
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetTileAccessViolationTileMatrixIncorrect()
    {
        var result = await TestProviders.GetControllerWithAccessDelegate().GetTile("data", 8, 82, 162, "qwerty");
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetTileAccessViolationTileRowIncorrect()
    {
        var result = await TestProviders.GetControllerWithAccessDelegate().GetTile("data", 7, 41, 81, "qwerty");
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetTileAccessViolationTileColIncorrect()
    {
        var result = await TestProviders.GetControllerWithAccessDelegate().GetTile("data", 7, 40, 80, "qwerty");
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task GetTileAccessOk()
    {
        var result = await TestProviders.GetControllerWithAccessDelegate().GetTile("data", 7, 40, 81, apiKey: "qwerty");
        Assert.IsType<FileContentResult>(result);
    }

    [Fact]
    public async Task GetTileAccessDelegateNotSet()
    {
        var result = await TestProviders.GetControllerWithoutAccessDelegate().GetTile("data", 7, 40, 81, apiKey: "qwerty");
        Assert.IsType<FileContentResult>(result);
    }

    [Fact]
    public async Task GetTileDirectFileNotExists()
    {
        await Assert.ThrowsAsync<SqliteException>(() => MbTilesProvider.GetTileDirectAsync(Path.Combine("Data", "test.mbtiles"), 8, 82, 162));
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

    [Fact]
    public void GetLimitsWithMinMaxZoom()
    {
        var limits = TestProviders.GetProviderWithMinMaxZoom().GetLimits("data");
        Assert.Equal(5, limits.Count);
    }

    [Fact]
    public async Task GetTileFeatureAccessApiKeyNull()
    {
        var result = await TestProviders.GetControllerWithFeatureAccessDelegate().GetTile("featureAccessData", 0, 0, 0);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetTileFeatureAccessApiKeyAdmin()
    {
        const int countLayers = 2;
        var featuresCount = new Dictionary<string, int>
        {
            {"chess", 3968 },
            {"circle", 3968 }
        };

        var result = await TestProviders.GetControllerWithFeatureAccessDelegate().GetTile("featureAccessData", 0, 0, 0, apiKey: "admin");
        Assert.IsType<FileContentResult>(result);

        var fileContent = result as FileContentResult;
        Assert.NotNull(fileContent);

        var reader = new MapboxTileReader();
        await using var memoryStream = new MemoryStream(fileContent.FileContents);
        await using var decompressor = new GZipStream(memoryStream, CompressionMode.Decompress, false);
        var tile = reader.Read(decompressor, new NetTopologySuite.IO.VectorTiles.Tiles.Tile(0, 0, 0));

        Assert.Equal(countLayers, tile.Layers.Count);

        foreach (var layer in tile.Layers)
            Assert.Equal(featuresCount[layer.Name], layer.Features.Count);
    }

    [Fact]
    public async Task GetTileFeatureAccessApiKeyUser1()
    {
        const int countLayers = 2;
        var featuresCount = new Dictionary<string, int>
        {
            {"chess", 1984 },
            {"circle", 179 }
        };

        var result = await TestProviders.GetControllerWithFeatureAccessDelegate().GetTile("featureAccessData", 0, 0, 0, apiKey: "user1");
        Assert.IsType<FileContentResult>(result);

        var fileContent = result as FileContentResult;
        Assert.NotNull(fileContent);

        var reader = new MapboxTileReader();
        await using var memoryStream = new MemoryStream(fileContent.FileContents);
        await using var decompressor = new GZipStream(memoryStream, CompressionMode.Decompress, false);
        var tile = reader.Read(decompressor, new NetTopologySuite.IO.VectorTiles.Tiles.Tile(0, 0, 0));

        Assert.Equal(countLayers, tile.Layers.Count);

        foreach (var layer in tile.Layers)
            Assert.Equal(featuresCount[layer.Name], layer.Features.Count);
    }

    [Fact]
    public async Task GetTileFeatureAccessApiKeyUser2()
    {
        const int countLayers = 2;
        var featuresCount = new Dictionary<string, int>
        {
            {"chess", 1984 },
            {"circle", 3789 }
        };

        var result = await TestProviders.GetControllerWithFeatureAccessDelegate().GetTile("featureAccessData", 0, 0, 0, apiKey: "user2");
        Assert.IsType<FileContentResult>(result);

        var fileContent = result as FileContentResult;
        Assert.NotNull(fileContent);

        var reader = new MapboxTileReader();
        await using var memoryStream = new MemoryStream(fileContent.FileContents);
        await using var decompressor = new GZipStream(memoryStream, CompressionMode.Decompress, false);
        var tile = reader.Read(decompressor, new NetTopologySuite.IO.VectorTiles.Tiles.Tile(0, 0, 0));

        Assert.Equal(countLayers, tile.Layers.Count);

        foreach (var layer in tile.Layers)
            Assert.Equal(featuresCount[layer.Name], layer.Features.Count);
    }

    [Fact]
    public async Task GetTileFeatureAccessApiKeyValue()
    {
        const int countLayers = 2;
        var featuresCount = new Dictionary<string, int>
        {
            {"chess", 1327 },
            {"circle", 2027 }
        };

        var result = await TestProviders.GetControllerWithFeatureAccessDelegate().GetTile("featureAccessData", 0, 0, 0, apiKey: "value");
        Assert.IsType<FileContentResult>(result);

        var fileContent = result as FileContentResult;
        Assert.NotNull(fileContent);

        var reader = new MapboxTileReader();
        await using var memoryStream = new MemoryStream(fileContent.FileContents);
        await using var decompressor = new GZipStream(memoryStream, CompressionMode.Decompress, false);
        var tile = reader.Read(decompressor, new NetTopologySuite.IO.VectorTiles.Tiles.Tile(0, 0, 0));

        Assert.Equal(countLayers, tile.Layers.Count);

        foreach (var layer in tile.Layers)
            Assert.Equal(featuresCount[layer.Name], layer.Features.Count);
    }

    [Fact]
    public void GetPropertyMetadata()
    {
        var expected = new Dictionary<string, string>
        {
            { "fid", "number" },
            { "geometry", "polygon" },
            { "name", "string" },
            { "value", "number" }
        };

        var actual = TestProviders.GetDefaultProvider().GetPropertyMetadata("data");

        Assert.Equal(expected, actual);
    }
}