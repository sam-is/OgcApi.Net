using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NetTopologySuite.Geometries;
using OgcApi.Features.SqlServer.Tests.Utils;
using OgcApi.Net.Features.SqlServer;
using OgcApi.Net.Features.SqlServer.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace OgcApi.Features.SqlServer.Tests
{
    public class SqlServerFacts : IClassFixture<DatabaseFixture>
    {
        [Fact]
        public void DatabaseCreation()
        {
            using var sqlConnection = new SqlConnection(DatabaseUtils.GetConnectionString());
            sqlConnection.Open();

            using var sqlCommand = new SqlCommand("SELECT DB_ID(@DatabaseName)", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@DatabaseName", DatabaseUtils.DatabaseName);
            var databaseId = sqlCommand.ExecuteScalar();
            Assert.IsNotType<DBNull>(databaseId);
        }

        [Fact]
        public void ConstructorValid()
        {
            Assert.NotNull(TestProviders.GetDefaultProvider());
        }

        [Fact]
        public void ConstructorWrongOptions()
        {
            var options = new SqlServerCollectionSourcesOptions()
            {
                Sources = new List<SqlServerCollectionSourceOptions>()
                {
                    new SqlServerCollectionSourceOptions()
                    {
                        Id = "Polygons"
                    }
                }
            };
            var optionsMonitor = Mock.Of<IOptionsMonitor<SqlServerCollectionSourcesOptions>>(mock => mock.CurrentValue == options);

            Assert.Throws<OptionsValidationException>(() => new SqlServerProvider(optionsMonitor, new NullLogger<SqlServerProvider>()));
        }

        [Fact]
        public void ConstructorNullOptions()
        {
            Assert.Throws<ArgumentNullException>(() => new SqlServerProvider(null, new NullLogger<SqlServerProvider>()));
        }

        [Fact]
        public void GetFeature()
        {
            var feature = TestProviders.GetDefaultProvider().GetFeature("Polygons", "1");

            Assert.NotNull(feature);
            Assert.Equal("POLYGON ((0 0, 0 1000000, 1000000 1000000, 1000000 0, 0 0))", feature.Geometry.ToString());
            Assert.Equal("Simple polygon", (string)feature.Attributes["Name"]);
            Assert.Equal(1, (int)feature.Attributes["Number"]);
            Assert.Equal(0.25, (double)feature.Attributes["S"]);
            Assert.Equal(new DateTime(2020, 1, 1), (DateTime)feature.Attributes["Date"]);
        }

        [Fact]
        public void GetFeatureTableNotExists()
        {
            Assert.Throws<SqlException>(() => TestProviders.GetProviderWithErrors().GetFeature("Test", "1"));
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
            Assert.Throws<SqlException>(() => TestProviders.GetProviderWithErrors().GetBbox("Test"));
        }

        [Fact]
        public void GetFeaturesDefaultParameters()
        {
            var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons");

            Assert.Equal(4, features.Count);
            Assert.Equal("POLYGON ((0 0, 0 1000000, 1000000 1000000, 1000000 0, 0 0))", features[0].Geometry.ToString());
            Assert.Equal("POLYGON ((2000000 0, 2000000 1000000, 3000000 1000000, 3000000 0, 2000000 0), (2250000 250000, 2250000 750000, 2750000 750000, 2750000 250000, 2250000 250000))", features[1].Geometry.ToString());
            Assert.Equal("MULTIPOLYGON (((0 2000000, 0 3000000, 1000000 3000000, 1000000 2000000, 0 2000000)), ((1250000 2250000, 1250000 2750000, 1750000 2750000, 1750000 2250000, 1250000 2250000)))", features[2].Geometry.ToString());
            Assert.Equal("MULTIPOLYGON (((2000000 2000000, 2000000 3000000, 3000000 3000000, 3000000 2000000, 2000000 2000000), (2250000 2250000, 2250000 2750000, 2750000 2750000, 2750000 2250000, 2250000 2250000)), ((3250000 2250000, 3250000 2750000, 3750000 2750000, 3750000 2250000, 3250000 2250000)))", features[3].Geometry.ToString());
        }

        [Fact]
        public void GetFeaturesLineStrings()
        {
            var features = TestProviders.GetDefaultProvider().GetFeatures("LineStrings");

            Assert.Equal(2, features.Count);
            Assert.Equal("LINESTRING (4000000 0, 4000000 1000000)", features[0].Geometry.ToString());
            Assert.Equal("MULTILINESTRING ((4000000 2000000, 4000000 3000000), (5000000 2000000, 5000000 3000000))", features[1].Geometry.ToString());
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
            var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", limit: 2);

            Assert.Equal(2, features.Count);
            Assert.Equal("Simple polygon", features[0].Attributes["Name"]);
            Assert.Equal("Polygon with hole", features[1].Attributes["Name"]);
        }

        [Fact]
        public void GetFeaturesLimitWithOffset()
        {
            var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", limit: 2, offset: 1);

            Assert.Equal(2, features.Count);
            Assert.Equal("Polygon with hole", features[0].Attributes["Name"]);
            Assert.Equal("MiltiPolygon with two parts", features[1].Attributes["Name"]);
        }

        [Fact]
        public void GetFeaturesBbox()
        {
            var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", bbox: new Envelope(0, 3000000, 0, 1000000));

            Assert.Equal(2, features.Count);
            Assert.Equal("Simple polygon", features[0].Attributes["Name"]);
            Assert.Equal("Polygon with hole", features[1].Attributes["Name"]);
        }

        [Fact]
        public void GetFeaturesStartDate()
        {
            var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", startDateTime: new DateTime(2022, 1, 1));

            Assert.Equal(2, features.Count);
            Assert.Equal("MiltiPolygon with two parts", features[0].Attributes["Name"]);
            Assert.Equal("MiltiPolygon with two parts, one with hole", features[1].Attributes["Name"]);
        }

        [Fact]
        public void GetFeaturesEndDate()
        {
            var features = TestProviders.GetDefaultProvider().GetFeatures("Polygons", endDateTime: new DateTime(2022, 1, 1));

            Assert.Equal(3, features.Count);
            Assert.Equal("Simple polygon", features[0].Attributes["Name"]);
            Assert.Equal("Polygon with hole", features[1].Attributes["Name"]);
            Assert.Equal("MiltiPolygon with two parts", features[2].Attributes["Name"]);
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
            Assert.Equal("MiltiPolygon with two parts", features[1].Attributes["Name"]);
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

        [Fact]
        public void GetFeaturesWithoutApiKey()
        {
            Assert.Throws<ArgumentException>(() => TestProviders.GetProviderWithApiKey().GetFeatures("PointsWithApiKey"));
        }

        [Fact]
        public void GetFeaturesTableNotExists()
        {
            Assert.Throws<SqlException>(() => TestProviders.GetProviderWithErrors().GetFeatures("Test"));
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
            Assert.Throws<SqlException>(() => TestProviders.GetProviderWithErrors().GetFeaturesCount("Test"));
        }

        [Fact]
        public void GetFeaturesCountBbox()
        {
            Assert.Equal(2, TestProviders.GetDefaultProvider().GetFeaturesCount("Polygons", bbox: new Envelope(0, 3000000, 0, 1000000)));
        }

        [Fact]
        public void GetFeaturesCountStartDate()
        {
            Assert.Equal(2, TestProviders.GetDefaultProvider().GetFeaturesCount("Polygons", startDateTime: new DateTime(2022, 1, 1)));
        }

        [Fact]
        public void GetFeaturesCountEndDate()
        {
            Assert.Equal(3, TestProviders.GetDefaultProvider().GetFeaturesCount("Polygons", endDateTime: new DateTime(2022, 1, 1)));
        }

        [Fact]
        public void GetFeaturesCountStartAndEndDate()
        {
            Assert.Equal(2, TestProviders.GetDefaultProvider().GetFeaturesCount(
                "Polygons",
                startDateTime: new DateTime(2021, 1, 1),
                endDateTime: new DateTime(2022, 1, 1)));
        }
    }
}
