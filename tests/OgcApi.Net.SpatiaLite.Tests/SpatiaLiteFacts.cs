using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.VectorTiles.Mapbox;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Features;
using OgcApi.Net.Options;
using OgcApi.SpatiaLite.Tests.Utils;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using Xunit;
using OgcApi.Net.SpatiaLite;
using System.Threading.Tasks;

namespace OgcApi.SpatiaLite.Tests;

public class SpatiaLiteFacts : IClassFixture<DatabaseFixture>
{
    [Fact]
    public void DatabaseCreation()
    {
        using var sqlConnection = new SpatiaLiteConnection(DatabaseUtils.GetConnectionString());
        sqlConnection.Open();

        using var sqlCommand = new SqliteCommand("SELECT name FROM sqlite_schema WHERE type='table' AND name=@tableName;", sqlConnection);
        sqlCommand.Parameters.AddWithValue("@tableName", "Polygons");
        var tableName = sqlCommand.ExecuteScalar();
        Assert.IsNotType<DBNull>(tableName);
    }
    [Fact]
    public void ConstructorValid()
    {
        Assert.NotNull(TestProviders.GetDefaultProvider());
    }
    [Fact]
    public void ConstructorWrongOptions()
    {
        var options = new CollectionsOptions
        {
            Items =
            [
                new CollectionOptions
                {
                    Id = "Polygons"
                }
            ]
        };

        Assert.Throws<OptionsValidationException>(() => CollectionsOptionsValidator.Validate(options));
    }
    [Fact]
    public void ConstructorNullOptions()
    {
        Assert.Throws<OptionsValidationException>(() => CollectionsOptionsValidator.Validate(null));
    }
    [Fact]
    public void GetFeature()
    {
        var feature = TestProviders.GetDefaultProvider().GetFeature("Polygons", "1");

        Assert.NotNull(feature);
        Assert.Equal("POLYGON ((0 0, 0 1000000, 1000000 1000000, 1000000 0, 0 0))", feature.Geometry.ToString());
        Assert.Equal("Simple polygon", (string)feature.Attributes["Name"]);
        Assert.Equal(1, (long)feature.Attributes["Number"]);
        Assert.Equal(0.25, (double)feature.Attributes["S"]);
        Assert.Equal(new DateTime(2020, 1, 1), DateTime.Parse((string)feature.Attributes["Date"]));
    }

    [Fact]
    public void GetFeatureTableNotExists()
    {
        Assert.Throws<SqliteException>(() => TestProviders.GetProviderWithErrors().GetFeature("Test", "1"));
    }

    [Fact]
    public void GetFeatureUnknownCollection()
    {
        Assert.Throws<ArgumentException>(() => TestProviders.GetDefaultProvider().GetFeature("Test", "1"));
    }

    [Fact]
    public void GetFeatureIdDoesNotExists()
    {
        Assert.Null(TestProviders.GetDefaultProvider().GetFeature("Polygons", "0"));
    }

    [Fact]
    public void GetBbox()
    {
        var bbox = TestProviders.GetDefaultProvider().GetBbox("Polygons");
        Assert.Equal("Env[0 : 3750000, 0 : 3000000]", bbox.ToString());
    }

    [Fact]
    public void GetBboxEmptyTable()
    {
        Assert.Null(TestProviders.GetDefaultProvider().GetBbox("Empty"));
    }

    [Fact]
    public void GetBboxUnknownCollection()
    {
        Assert.Throws<ArgumentException>(() => TestProviders.GetDefaultProvider().GetBbox("Test"));
    }

    [Fact]
    public void GetBboxTableNotExists()
    {
        Assert.Throws<SqliteException>(() => TestProviders.GetProviderWithErrors().GetBbox("Test"));
    }

    [Fact]
    public void GetFeaturesDefaultParameters()
    {
        var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons");

        Assert.Equal(4, features.Count);
        Assert.Equal("POLYGON ((0 0, 0 1000000, 1000000 1000000, 1000000 0, 0 0))", features[0].Geometry.ToString());
        Assert.Equal(
            "POLYGON ((2000000 0, 2000000 1000000, 3000000 1000000, 3000000 0, 2000000 0), (2250000 250000, 2250000 750000, 2750000 750000, 2750000 250000, 2250000 250000))",
            features[1].Geometry.ToString());
        Assert.Equal(
            "MULTIPOLYGON (((0 2000000, 0 3000000, 1000000 3000000, 1000000 2000000, 0 2000000)), ((1250000 2250000, 1250000 2750000, 1750000 2750000, 1750000 2250000, 1250000 2250000)))",
            features[2].Geometry.ToString());
        Assert.Equal(
            "MULTIPOLYGON (((2000000 2000000, 2000000 3000000, 3000000 3000000, 3000000 2000000, 2000000 2000000), (2250000 2250000, 2250000 2750000, 2750000 2750000, 2750000 2250000, 2250000 2250000)), ((3250000 2250000, 3250000 2750000, 3750000 2750000, 3750000 2250000, 3250000 2250000)))",
            features[3].Geometry.ToString());
    }

    [Fact]
    public void GetFeaturesLineStrings()
    {
        var features = TestProviders.GetDefaultProvider().GetFeatures("LineStrings");

        Assert.Equal(2, features.Count);
        Assert.Equal("LINESTRING (4000000 0, 4000000 1000000)", features[0].Geometry.ToString());
        Assert.Equal("MULTILINESTRING ((4000000 2000000, 4000000 3000000), (5000000 2000000, 5000000 3000000))",
            features[1].Geometry.ToString());
    }

    [Fact]
    public void GetFeaturesPoints()
    {
        var features = TestProviders.GetDefaultProvider().GetFeatures("Points");

        Assert.Equal(2, features.Count);
        Assert.Equal("POINT (500000 500000)", features[0].Geometry.ToString());
        Assert.Equal("MULTIPOINT ((500000 2500000), (2500000 500000), (2500000 2500000))", features[1].Geometry.ToString());
    }

    [Fact]
    public void GetFeaturesLimit()
    {
        var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", 2);

        Assert.Equal(2, features.Count);
        Assert.Equal("Simple polygon", features[0].Attributes["Name"]);
        Assert.Equal("Polygon with hole", features[1].Attributes["Name"]);
    }

    [Fact]
    public void GetFeaturesLimitWithOffset()
    {
        var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", 2, 1);

        Assert.Equal(2, features.Count);
        Assert.Equal("Polygon with hole", features[0].Attributes["Name"]);
        Assert.Equal("MultiPolygon with two parts", features[1].Attributes["Name"]);
    }

    [Fact]
    public void GetFeaturesBbox()
    {
        var features = TestProviders.GetDefaultProvider()
            .GetFeatures("Polygons", bbox: new Envelope(0, 3000000, 0, 1000000));

        Assert.Equal(2, features.Count);
        Assert.Equal("Simple polygon", features[0].Attributes["Name"]);
        Assert.Equal("Polygon with hole", features[1].Attributes["Name"]);
    }

    [Fact]
    public void GetFeaturesStartDate()
    {
        var features = TestProviders.GetDefaultProvider()
            .GetFeatures("Polygons", startDateTime: new DateTime(2022, 1, 1));

        Assert.Equal(2, features.Count);
        Assert.Equal("MultiPolygon with two parts", features[0].Attributes["Name"]);
        Assert.Equal("MultiPolygon with two parts, one with hole", features[1].Attributes["Name"]);
    }

    [Fact]
    public void GetFeaturesEndDate()
    {
        var features = TestProviders.GetDefaultProvider()
            .GetFeatures("Polygons", endDateTime: new DateTime(2022, 1, 1));

        Assert.Equal(3, features.Count);
        Assert.Equal("Simple polygon", features[0].Attributes["Name"]);
        Assert.Equal("Polygon with hole", features[1].Attributes["Name"]);
        Assert.Equal("MultiPolygon with two parts", features[2].Attributes["Name"]);
    }

    [Fact]
    public void GetFeaturesStartAndEndDate()
    {
        var features = TestProviders.GetDefaultProvider().GetFeatures(
            "Polygons",
            startDateTime: new DateTime(2021, 1, 1),
            endDateTime: new DateTime(2022, 1, 1));

        Assert.Equal(2, features.Count);
        Assert.Equal("Polygon with hole", features[0].Attributes["Name"]);
        Assert.Equal("MultiPolygon with two parts", features[1].Attributes["Name"]);
    }

    [Fact]
    public void GetFeaturesWrongLimit()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => TestProviders.GetDefaultProvider().GetFeatures("Polygons", limit: 0));
    }

    [Fact]
    public void GetFeaturesUnknownCollection()
    {
        Assert.Throws<ArgumentException>(() => TestProviders.GetDefaultProvider().GetFeatures("Test"));
    }

    [Fact]
    public void GetFeaturesApiKey()
    {
        var features = TestProviders.GetProviderWithApiKey().GetFeatures("PointsWithApiKey", apiKey: "2");

        Assert.Equal(3, features.Count);
        Assert.Equal("Point 2", features[0].Attributes["Name"]);
        Assert.Equal("Point 3", features[1].Attributes["Name"]);
        Assert.Equal("Point 4", features[2].Attributes["Name"]);
    }

    [Theory]
    [InlineData(["S", "0.25"])]
    [InlineData(["Name", "Simple polygon"])]
    public void GetFeaturesPropertyFilter(string key, string value)
    {
        var filter = new Dictionary<string, string>() { { key, value } };
        var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", propertyFilter: filter);

        Assert.Single(features);
        Assert.NotNull(features[0]);
        Assert.Equal("Simple polygon", features[0].Attributes["Name"]);
        Assert.Equal(0.25, (double)features[0].Attributes["S"]);
    }

    [Fact]
    public void GetFeaturesWithoutApiKey()
    {
        Assert.Throws<UnauthorizedAccessException>(() => TestProviders.GetProviderWithApiKey().GetFeatures("PointsWithApiKey"));
    }

    [Fact]
    public void GetFeaturesTableNotExists()
    {
        Assert.Throws<SqliteException>(() => TestProviders.GetProviderWithErrors().GetFeatures("Test"));
    }

    [Fact]
    public void GetFeaturesCount()
    {
        Assert.Equal(4, TestProviders.GetDefaultProvider().GetFeaturesCount("Polygons"));
    }

    [Fact]
    public void GetFeaturesCountEmptyTable()
    {
        Assert.Equal(0, TestProviders.GetDefaultProvider().GetFeaturesCount("Empty"));
    }

    [Fact]
    public void GetFeaturesCountUnknownCollection()
    {
        Assert.Throws<ArgumentException>(() => TestProviders.GetDefaultProvider().GetFeaturesCount("Test"));
    }

    [Fact]
    public void GetFeaturesCountTableNotExists()
    {
        Assert.Throws<SqliteException>(() => TestProviders.GetProviderWithErrors().GetFeaturesCount("Test"));
    }

    [Fact]
    public void GetFeaturesCountBbox()
    {
        Assert.Equal(2,
            TestProviders.GetDefaultProvider()
                .GetFeaturesCount("Polygons", bbox: new Envelope(0, 3000000, 0, 1000000)));
    }

    [Fact]
    public void GetFeaturesCountStartDate()
    {
        Assert.Equal(2,
            TestProviders.GetDefaultProvider()
                .GetFeaturesCount("Polygons", startDateTime: new DateTime(2022, 1, 1)));
    }

    [Fact]
    public void GetFeaturesCountEndDate()
    {
        Assert.Equal(3,
            TestProviders.GetDefaultProvider().GetFeaturesCount("Polygons", endDateTime: new DateTime(2022, 1, 1)));
    }

    [Fact]
    public void GetFeaturesCountStartAndEndDate()
    {
        Assert.Equal(2, TestProviders.GetDefaultProvider().GetFeaturesCount(
            "Polygons",
            startDateTime: new DateTime(2021, 1, 1),
            endDateTime: new DateTime(2022, 1, 1)));
    }

    private static OgcFeature CreateTestFeature(IFeaturesProvider provider)
    {
        var feature =
            new OgcFeature
            {
                Attributes = new AttributesTable(
                    new Dictionary<string, object>
                    {
                        { "Name", "CreateTest" },
                        { "Number", 1L },
                        { "Date", new DateTime(2021, 1, 1) },
                        { "S", 0.25 }
                    }
                ),
                Geometry = new Polygon(
                    new LinearRing(
                        [
                            new Coordinate(0, 0),
                            new Coordinate(0, 1000000),
                            new Coordinate(1000000, 1000000),
                            new Coordinate(1000000, 0),
                            new Coordinate(0, 0)
                        ]
                    )
                )
            };
        feature.Id = provider.CreateFeature("PolygonsForInsert", feature);
        return feature;
    }

    private static void DeleteTestFeature(IFeaturesProvider provider, string featureId)
    {
        provider.DeleteFeature("PolygonsForInsert", featureId);
    }

    [Fact]
    public void CreateFeature()
    {
        var provider = TestProviders.GetDefaultProvider();

        var testFeature = CreateTestFeature(provider);
        Assert.NotNull(testFeature.Id);

        var insertedFeature = provider.GetFeature("PolygonsForInsert", (string)testFeature.Id);
        Assert.Equal(testFeature.Geometry, insertedFeature.Geometry);
        Assert.Equal(testFeature.Attributes["Name"], insertedFeature.Attributes["Name"]);
        Assert.Equal(testFeature.Attributes["Number"], insertedFeature.Attributes["Number"]);
        Assert.Equal(testFeature.Attributes["Date"], DateTime.Parse((string)insertedFeature.Attributes["Date"]));

        DeleteTestFeature(provider, (string)testFeature.Id);
    }

    [Fact]
    public void CreateFeatureNullGeometry()
    {
        var provider = TestProviders.GetDefaultProvider();

        var testFeature =
            new OgcFeature
            {
                Attributes = new AttributesTable(
                    new Dictionary<string, object>
                    {
                        { "Name", "CreateTest" },
                        { "Number", 1 },
                        { "Date", new DateTime(2021, 1, 1) }
                    }
                )
            };
        Assert.Throws<ArgumentException>(() => provider.CreateFeature("PolygonsForInsert", testFeature));
    }

    [Fact]
    public void UpdateFeature()
    {
        var provider = TestProviders.GetDefaultProvider();

        var testFeature = CreateTestFeature(provider);
        var testFeatureId = (string)testFeature.Id;

        var featureUpdateFrom =
            new OgcFeature
            {
                Attributes = new AttributesTable(
                    new Dictionary<string, object>
                    {
                        { "Name", "UpdateTest" },
                        { "Number", 11L },
                        { "Date", new DateTime(2021, 1, 2) }
                    }
                ),
                Geometry = new Polygon(
                    new LinearRing(
                        [
                            new Coordinate(0, 0),
                            new Coordinate(0, 1000001),
                            new Coordinate(1000001, 1000001),
                            new Coordinate(1000001, 0),
                            new Coordinate(0, 0)
                        ]
                    )
                )
            };
        provider.UpdateFeature("PolygonsForInsert", testFeatureId, featureUpdateFrom);

        var updatedFeature = provider.GetFeature("PolygonsForInsert", testFeatureId);
        Assert.Equal(featureUpdateFrom.Geometry, updatedFeature.Geometry);
        Assert.Equal(featureUpdateFrom.Attributes["Name"], updatedFeature.Attributes["Name"]);
        Assert.Equal(featureUpdateFrom.Attributes["Number"], updatedFeature.Attributes["Number"]);
        Assert.Equal(featureUpdateFrom.Attributes["Date"], DateTime.Parse((string)updatedFeature.Attributes["Date"]));
        Assert.Equal(0.25, updatedFeature.Attributes["S"]);

        DeleteTestFeature(provider, testFeatureId);
    }

    [Fact]
    public void UpdateFeatureNullGeometry()
    {
        var provider = TestProviders.GetDefaultProvider();

        var testFeature = CreateTestFeature(provider);
        var testFeatureId = (string)testFeature.Id;

        var featureBeforeUpdate = provider.GetFeature("PolygonsForInsert", testFeatureId);

        var featureUpdateFrom =
            new OgcFeature
            {
                Attributes = new AttributesTable(
                    new Dictionary<string, object>
                    {
                        { "Name", "UpdateTest" }
                    }
                )
            };
        provider.UpdateFeature("PolygonsForInsert", testFeatureId, featureUpdateFrom);

        var updatedFeature = provider.GetFeature("PolygonsForInsert", testFeatureId);

        Assert.Equal(featureBeforeUpdate.Geometry, updatedFeature.Geometry);
        Assert.Equal(featureUpdateFrom.Attributes["Name"], updatedFeature.Attributes["Name"]);
        Assert.Equal(featureBeforeUpdate.Attributes["Number"], updatedFeature.Attributes["Number"]);
        Assert.Equal(featureBeforeUpdate.Attributes["Date"], updatedFeature.Attributes["Date"]);
        Assert.Equal(featureBeforeUpdate.Attributes["S"], updatedFeature.Attributes["S"]);

        DeleteTestFeature(provider, testFeatureId);
    }

    [Fact]
    public void UpdateFeatureOnlyGeometry()
    {
        var provider = TestProviders.GetDefaultProvider();

        var testFeature = CreateTestFeature(provider);
        var testFeatureId = (string)testFeature.Id;

        var featureBeforeUpdate = provider.GetFeature("PolygonsForInsert", testFeatureId);

        var featureUpdateFrom =
            new OgcFeature
            {
                Geometry = new Polygon(
                    new LinearRing(
                        [
                            new Coordinate(0, 0),
                            new Coordinate(0, 1000002),
                            new Coordinate(1000002, 1000002),
                            new Coordinate(1000002, 0),
                            new Coordinate(0, 0)
                        ]
                    )
                )
            };
        provider.UpdateFeature("PolygonsForInsert", testFeatureId, featureUpdateFrom);

        var updatedFeature = provider.GetFeature("PolygonsForInsert", testFeatureId);
        Assert.Equal(featureUpdateFrom.Geometry, updatedFeature.Geometry);
        Assert.Equal(featureBeforeUpdate.Attributes["Name"], updatedFeature.Attributes["Name"]);
        Assert.Equal(featureBeforeUpdate.Attributes["Number"], updatedFeature.Attributes["Number"]);
        Assert.Equal(featureBeforeUpdate.Attributes["Date"], updatedFeature.Attributes["Date"]);
        Assert.Equal(featureBeforeUpdate.Attributes["S"], updatedFeature.Attributes["S"]);

        DeleteTestFeature(provider, testFeatureId);
    }

    [Fact]
    public void ReplaceFeature()
    {
        var provider = TestProviders.GetDefaultProvider();

        var testFeature = CreateTestFeature(provider);
        var testFeatureId = (string)testFeature.Id;

        var featureReplaceFrom =
            new OgcFeature
            {
                Attributes = new AttributesTable(
                    new Dictionary<string, object>
                    {
                        { "Name", "UpdateTest" },
                        { "Number", 11L },
                        { "Date", new DateTime(2021, 1, 1) }
                    }
                ),
                Geometry = new Polygon(
                    new LinearRing(
                        [
                            new Coordinate(0, 0),
                            new Coordinate(0, 1000001),
                            new Coordinate(1000001, 1000001),
                            new Coordinate(1000001, 0),
                            new Coordinate(0, 0)
                        ]
                    )
                )
            };
        provider.ReplaceFeature("PolygonsForInsert", testFeatureId, featureReplaceFrom);

        var updatedFeature = provider.GetFeature("PolygonsForInsert", testFeatureId);
        Assert.Equal(featureReplaceFrom.Geometry, updatedFeature.Geometry);
        Assert.Equal(featureReplaceFrom.Attributes["Name"], updatedFeature.Attributes["Name"]);
        Assert.Equal(featureReplaceFrom.Attributes["Number"], updatedFeature.Attributes["Number"]);
        Assert.Equal(featureReplaceFrom.Attributes["Date"], DateTime.Parse((string)updatedFeature.Attributes["Date"]));
        Assert.True(!updatedFeature.Attributes.GetNames().Contains("S"));

        DeleteTestFeature(provider, testFeatureId);
    }

    [Fact]
    public void ReplaceFeatureNullGeometry()
    {
        var provider = TestProviders.GetDefaultProvider();

        var feature =
            new OgcFeature
            {
                Attributes = new AttributesTable(
                    new Dictionary<string, object>
                    {
                        { "Name", "UpdateTest" }
                    }
                )
            };
        Assert.Throws<ArgumentException>(() => provider.ReplaceFeature("PolygonsForInsert", "1", feature));
    }

    [Fact]
    public void DeleteFeature()
    {
        var provider = TestProviders.GetDefaultProvider();

        var testFeature = CreateTestFeature(provider);
        var testFeatureId = (string)testFeature.Id;

        DeleteTestFeature(provider, testFeatureId);
        Assert.Null(provider.GetFeature("PolygonsForInsert", testFeatureId));
    }

    [Fact]
    public void DeleteNotExistingFeature()
    {
        var provider = TestProviders.GetDefaultProvider();
        Assert.Throws<ArgumentException>(() => provider.DeleteFeature("PolygonsForInsert", "200"));
    }

    [Fact]
    public async Task GetTile()
    {
        var tile = await TestProviders.GetDefaultProvider().GetTileAsync("Polygons", 1, 0, 1);
        Assert.NotNull(tile);
    }

    [Fact]
    public async Task GetTileUnknownCollection()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => TestProviders.GetDefaultProvider().GetTileAsync("test", 1, 1, 1));
    }

    [Fact]
    public async Task GetTileEmptyTile()
    {
        var rawTile = await TestProviders.GetDefaultProvider().GetTileAsync("Polygons", 8, 1, 250);
        using var memoryStream = new MemoryStream(rawTile);
        memoryStream.Seek(0, SeekOrigin.Begin);

        await using var decompressor = new GZipStream(memoryStream, CompressionMode.Decompress);

        using var decompressedStream = new MemoryStream();
        await decompressor.CopyToAsync(decompressedStream);

        var reader = new MapboxTileReader();
        var tile = reader.Read(decompressedStream, new NetTopologySuite.IO.VectorTiles.Tiles.Tile(250, 1, 8));
        Assert.True(tile.IsEmpty);
    }

    [Fact]
    public void GetLimitsUnknownCollection()
    {
        Assert.Throws<ArgumentException>(() => TestProviders.GetDefaultProvider().GetLimits("test"));
    }

    [Fact]
    public void GetLimits()
    {
        var limits = TestProviders.GetDefaultProvider().GetLimits("Polygons");
        for (var i = 0; i <= 22; i++)
        {
            Assert.True(limits[i].TileMatrix == i && limits[i].MinTileCol == 0 &&
                        limits[i].MaxTileCol == (1 << i) - 1 && limits[i].MinTileRow == 0 && limits[i].MaxTileRow == (1 << i) - 1);
        }
    }
}
