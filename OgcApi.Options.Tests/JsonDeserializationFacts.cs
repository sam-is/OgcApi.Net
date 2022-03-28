using OgcApi.Net.Options.Features;
using OgcApi.Options.Tests.Utils;
using Xunit;

namespace OgcApi.Options.Tests
{
    public class JsonDeserializationFacts : IClassFixture<JsonOptionsFixture>
    {
        private readonly JsonOptionsFixture _fixture;
        public JsonDeserializationFacts(JsonOptionsFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void OgcApiOptionsDeserialization()
        {
            Assert.NotNull(_fixture.Options);
        }

        [Fact]
        public void LandingPageDeserialization()
        {
            Assert.NotNull(_fixture.Options.LandingPage);
            Assert.Equal("API Title", _fixture.Options.LandingPage.Title);
            Assert.Equal("API Description", _fixture.Options.LandingPage.Description);
            Assert.Equal("1.0", _fixture.Options.LandingPage.Version);
            Assert.Equal("API Contacts", _fixture.Options.LandingPage.ContactName);
            Assert.Equal("https://example.com/", _fixture.Options.LandingPage.ContactUrl.ToString());
            Assert.Equal("https://api.com/index.html", _fixture.Options.LandingPage.ApiDocumentPage.ToString());
            Assert.Equal("https://api.com/swagger.json", _fixture.Options.LandingPage.ApiDescriptionPage.ToString());
            Assert.Equal("API License", _fixture.Options.LandingPage.LicenseName);
            Assert.Equal("https://api.com/license.html", _fixture.Options.LandingPage.LicenseUrl.ToString());
            Assert.NotNull(_fixture.Options.LandingPage.Links);
            Assert.NotEmpty(_fixture.Options.LandingPage.Links);
            Assert.Equal(2, _fixture.Options.LandingPage.Links.Count);
            Assert.Equal("https://api.com/landing_page/link1.html", _fixture.Options.LandingPage.Links[0].Href.ToString());
            Assert.Equal("https://api.com/landing_page/link2.html", _fixture.Options.LandingPage.Links[1].Href.ToString());
        }

        [Fact]
        public void ConformanceDeserialization()
        {
            Assert.NotNull(_fixture.Options.Conformance);
            Assert.NotNull(_fixture.Options.Conformance.ConformsTo);
            Assert.NotEmpty(_fixture.Options.Conformance.ConformsTo);
            Assert.Equal(2, _fixture.Options.Conformance.ConformsTo.Count);
            Assert.Equal("https://api.com/conform1.html", _fixture.Options.Conformance.ConformsTo[0].ToString());
            Assert.Equal("https://api.com/conform2.html", _fixture.Options.Conformance.ConformsTo[1].ToString());
        }

        [Fact]
        public void UseApiKeyAuthorizationDeserialization()
        {
            Assert.True(_fixture.Options.UseApiKeyAuthorization);
        }

        [Fact]
        public void CollectionsLinksDeserialization()
        {
            Assert.NotNull(_fixture.Options.Collections);
            Assert.NotNull(_fixture.Options.Collections.Links);
            Assert.NotEmpty(_fixture.Options.Collections.Links);
            Assert.Equal(2, _fixture.Options.Collections.Links.Count);
            Assert.Equal("https://api.com/collections/link1.html", _fixture.Options.Collections.Links[0].Href.ToString());
            Assert.Equal("https://api.com/collections/link2.html", _fixture.Options.Collections.Links[1].Href.ToString());
        }

        [Fact]
        public void CollectionsItemsDeserialization()
        {
            Assert.NotNull(_fixture.Options.Collections);
            Assert.NotNull(_fixture.Options.Collections.Items);
            Assert.NotEmpty(_fixture.Options.Collections.Items);
            Assert.Equal(2, _fixture.Options.Collections.Items.Count);
        }

        [Fact]
        public void FirstCollectionOptionsDeserialization()
        {
            Assert.NotNull(_fixture.Options.Collections.Items[0]);
            Assert.Equal("Collection1", _fixture.Options.Collections.Items[0].Id);
            Assert.Equal("Collection title 1", _fixture.Options.Collections.Items[0].Title);
            Assert.Equal("Collection description 1", _fixture.Options.Collections.Items[0].Description);
            Assert.Equal("Collection1 ItemType", _fixture.Options.Collections.Items[0].ItemType);
            Assert.NotNull(_fixture.Options.Collections.Items[0].Features);
        }

        [Fact]
        public void SecondCollectionOptionsDeserialization()
        {
            Assert.NotNull(_fixture.Options.Collections.Items[1]);
            Assert.Equal("Collection2", _fixture.Options.Collections.Items[1].Id);
            Assert.Equal("Collection title 2", _fixture.Options.Collections.Items[1].Title);
            Assert.Equal("Collection description 2", _fixture.Options.Collections.Items[1].Description);
            Assert.Equal("Collection2 ItemType", _fixture.Options.Collections.Items[1].ItemType);
            Assert.NotNull(_fixture.Options.Collections.Items[1].Features);
        }

        [Fact]
        public void FirstCollectionFeaturesDeserialization()
        {
            Assert.NotNull(_fixture.Options.Collections.Items[0].Features);
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", _fixture.Options.Collections.Items[0].Features.StorageCrs.ToString());
            Assert.Equal("1", _fixture.Options.Collections.Items[0].Features.StorageCrsCoordinateEpoch);
            Assert.NotNull(_fixture.Options.Collections.Items[0].Features.Crs);
            Assert.NotEmpty(_fixture.Options.Collections.Items[0].Features.Crs);
            Assert.Equal(2, _fixture.Options.Collections.Items[0].Features.Crs.Count);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", _fixture.Options.Collections.Items[0].Features.Crs[0].ToString());
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", _fixture.Options.Collections.Items[0].Features.Crs[1].ToString());
            Assert.NotNull(_fixture.Options.Collections.Items[0].Features.Storage);

        }

        [Fact]
        public void SecondCollectionFeaturesDeserialization()
        {
            Assert.NotNull(_fixture.Options.Collections.Items[1].Features);
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", _fixture.Options.Collections.Items[1].Features.StorageCrs.ToString());
            Assert.Equal("2", _fixture.Options.Collections.Items[1].Features.StorageCrsCoordinateEpoch);
            Assert.NotNull(_fixture.Options.Collections.Items[1].Features.Crs);
            Assert.NotEmpty(_fixture.Options.Collections.Items[1].Features.Crs);
            Assert.Equal(2, _fixture.Options.Collections.Items[1].Features.Crs.Count);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", _fixture.Options.Collections.Items[1].Features.Crs[0].ToString());
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", _fixture.Options.Collections.Items[1].Features.Crs[1].ToString());
            Assert.NotNull(_fixture.Options.Collections.Items[1].Features.Storage);

        }

        [Fact]
        public void FirstCollectionStorageDeserialization()
        {
            var storage = _fixture.Options.Collections.Items[0].Features.Storage as SqlFeaturesSourceOptions;

            Assert.NotNull(storage);
            Assert.Equal("PostGis", storage.Type);
            Assert.Equal("Host=127.0.0.1;User Id=user;Password=user;Database=pgdb;Port=5432", storage.ConnectionString);
            Assert.Equal("api", storage.Schema);
            Assert.Equal("collection1", storage.Table);
            Assert.Equal("geom", storage.GeometryColumn);
            Assert.Equal(3857, storage.GeometrySrid);
            Assert.Equal("geometry", storage.GeometryDataType);
            Assert.Equal("MultiPolygon", storage.GeometryGeoJsonType);
            Assert.Equal("id", storage.IdentifierColumn);
            Assert.Equal("date", storage.DateTimeColumn);
            Assert.NotNull(storage.Properties);
            Assert.NotEmpty(storage.Properties);
            Assert.Equal(2, storage.Properties.Count);
            Assert.Equal("prop1", storage.Properties[0]);
            Assert.Equal("prop2", storage.Properties[1]);
            Assert.True(storage.AllowCreate);
            Assert.True(storage.AllowReplace);
            Assert.True(storage.AllowUpdate);
            Assert.True(storage.AllowDelete);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForGet);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForCreate);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForUpdate);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForDelete);
        }

        [Fact]
        public void SecondCollectionStorageDeserialization()
        {
            var storage = _fixture.Options.Collections.Items[1].Features.Storage as SqlFeaturesSourceOptions;

            Assert.NotNull(storage);
            Assert.Equal("SqlServer", storage.Type);
            Assert.Equal("data source=127.0.0.1,1433;user id=user;password=user;initial catalog=dbo;Persist Security Info=true", storage.ConnectionString);
            Assert.Equal("dbo", storage.Schema);
            Assert.Equal("Collection2", storage.Table);
            Assert.Equal("Geom", storage.GeometryColumn);
            Assert.Equal(3857, storage.GeometrySrid);
            Assert.Equal("geometry", storage.GeometryDataType);
            Assert.Equal("MultiPolygon", storage.GeometryGeoJsonType);
            Assert.Equal("Id", storage.IdentifierColumn);
            Assert.Equal("Date", storage.DateTimeColumn);
            Assert.NotNull(storage.Properties);
            Assert.NotEmpty(storage.Properties);
            Assert.Equal(2, storage.Properties.Count);
            Assert.Equal("Prop1", storage.Properties[0]);
            Assert.Equal("Prop2", storage.Properties[1]);
            Assert.True(storage.AllowCreate);
            Assert.True(storage.AllowReplace);
            Assert.True(storage.AllowUpdate);
            Assert.True(storage.AllowDelete);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForGet);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForCreate);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForUpdate);
            Assert.Equal("EXISTS(SELECT ... WHERE @ApiKey = apiKey)", storage.ApiKeyPredicateForDelete);
        }

        [Fact]
        public void FirstCollectionExtentDeserializationValid()
        {
            var extent = _fixture.Options.Collections.Items[0].Extent;

            Assert.NotNull(extent);
            Assert.NotNull(extent.Spatial);
            Assert.NotNull(extent.Spatial.Crs);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", extent.Spatial.Crs.ToString());
            Assert.NotNull(extent.Spatial.Bbox);
            Assert.NotEmpty(extent.Spatial.Bbox);
            Assert.Equal(2, extent.Spatial.Bbox.Length);
            Assert.NotEmpty(extent.Spatial.Bbox[0]);
            Assert.NotEmpty(extent.Spatial.Bbox[1]);
            Assert.Equal(2, extent.Spatial.Bbox[0].Length);
            Assert.Equal(2, extent.Spatial.Bbox[1].Length);
            Assert.Equal(1, extent.Spatial.Bbox[0][0]);
            Assert.Equal(2, extent.Spatial.Bbox[0][1]);
            Assert.Equal(3, extent.Spatial.Bbox[1][0]);
            Assert.Equal(4, extent.Spatial.Bbox[1][1]);
            Assert.NotNull(extent.Temporal);
            Assert.False(string.IsNullOrEmpty(extent.Temporal.Trs));
            Assert.Equal("Trs", extent.Temporal.Trs);
            Assert.NotNull(extent.Temporal.Interval);
            Assert.NotEmpty(extent.Temporal.Interval);
            Assert.Equal(2, extent.Temporal.Interval.Length);
            Assert.NotEmpty(extent.Temporal.Interval[0]);
            Assert.NotEmpty(extent.Temporal.Interval[1]);
            Assert.Equal(2, extent.Temporal.Interval[0].Length);
            Assert.Equal(2, extent.Temporal.Interval[1].Length);
            Assert.Equal(1, extent.Temporal.Interval[0][0]);
            Assert.Equal(2, extent.Temporal.Interval[0][1]);
            Assert.Equal(3, extent.Temporal.Interval[1][0]);
            Assert.Equal(4, extent.Temporal.Interval[1][1]);
        }

        [Fact]
        public void SecondCollectionExtentDeserializationValid()
        {
            var extent = _fixture.Options.Collections.Items[1].Extent;

            Assert.NotNull(extent);
            Assert.NotNull(extent.Spatial);
            Assert.NotNull(extent.Spatial.Crs);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", extent.Spatial.Crs.ToString());
            Assert.NotNull(extent.Spatial.Bbox);
            Assert.NotEmpty(extent.Spatial.Bbox);
            Assert.Equal(2, extent.Spatial.Bbox.Length);
            Assert.NotEmpty(extent.Spatial.Bbox[0]);
            Assert.NotEmpty(extent.Spatial.Bbox[1]);
            Assert.Equal(2, extent.Spatial.Bbox[0].Length);
            Assert.Equal(2, extent.Spatial.Bbox[1].Length);
            Assert.Equal(1, extent.Spatial.Bbox[0][0]);
            Assert.Equal(2, extent.Spatial.Bbox[0][1]);
            Assert.Equal(3, extent.Spatial.Bbox[1][0]);
            Assert.Equal(4, extent.Spatial.Bbox[1][1]);
            Assert.NotNull(extent.Temporal);
            Assert.False(string.IsNullOrEmpty(extent.Temporal.Trs));
            Assert.Equal("Trs", extent.Temporal.Trs);
            Assert.NotNull(extent.Temporal.Interval);
            Assert.NotEmpty(extent.Temporal.Interval);
            Assert.Equal(2, extent.Temporal.Interval.Length);
            Assert.NotEmpty(extent.Temporal.Interval[0]);
            Assert.NotEmpty(extent.Temporal.Interval[1]);
            Assert.Equal(2, extent.Temporal.Interval[0].Length);
            Assert.Equal(2, extent.Temporal.Interval[1].Length);
            Assert.Equal(1, extent.Temporal.Interval[0][0]);
            Assert.Equal(2, extent.Temporal.Interval[0][1]);
            Assert.Equal(3, extent.Temporal.Interval[1][0]);
            Assert.Equal(4, extent.Temporal.Interval[1][1]);
        }

        [Fact]
        public void DeserializeOptionsWithoutConformance()
        {
            OptionsUtils.SetupServiceCollection();
            Assert.NotNull(OptionsUtils.GetOptionsFromJsonWithoutConformance());
        }
    }
}