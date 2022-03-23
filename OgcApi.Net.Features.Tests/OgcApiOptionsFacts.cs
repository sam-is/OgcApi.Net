using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Options.SqlOptions;
using OgcApi.Net.Features.Tests.Utils;
using System.Linq;
using OgcApi.Net.Features.DataProviders;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
namespace OgcApi.Net.Features.Tests
{
    public class OgcApiOptionsSerializationFacts
    {
        public OgcApiOptionsSerializationFacts()
        {
            OptionsUtils.SetupServiceCollection();
        }

        [Fact]
        public void OgcApiOptionsSerialization()
        {
            var json = OptionsUtils.SerializeOgcApiOptions(OptionsUtils.GetOptionsFromCode());

            Assert.False(string.IsNullOrEmpty(json));
        }
        [Fact]
        public void LandingPageOptionsSerialization()
        {
            var json = OptionsUtils.SerializeLandingPageOptions(OptionsUtils.GetOptionsFromCode().LandingPage);

            Assert.False(string.IsNullOrEmpty(json));
        }
        [Fact]
        public void CollectionsOptionsSerialization()
        {
            var json = OptionsUtils.SerializeCollectionsOptions(OptionsUtils.GetOptionsFromCode().Collections);

            Assert.False(string.IsNullOrEmpty(json));
        }

    }

    public class JsonDeserializationFacts
    {
        readonly OgcApiOptions options;
        public JsonDeserializationFacts()
        {
            OptionsUtils.SetupServiceCollection();
            options = OptionsUtils.GetOptionsFromJson();
        }

        [Fact]
        public void OgcApiOptionsDeserialization()
        {
            Assert.NotNull(options);
        }

        [Fact]
        public void LandingPageDeserialization()
        {
            Assert.NotNull(options.LandingPage);
            Assert.Equal("API Title", options.LandingPage.Title);
            Assert.Equal("API Description", options.LandingPage.Description);
            Assert.Equal("1.0", options.LandingPage.Version);
            Assert.Equal("API Contacts", options.LandingPage.ContactName);
            Assert.Equal("https://example.com/", options.LandingPage.ContactUrl.ToString());
            Assert.Equal("https://api.com/index.html", options.LandingPage.ApiDocumentPage.ToString());
            Assert.Equal("https://api.com/swagger.json", options.LandingPage.ApiDescriptionPage.ToString());
            Assert.Equal("API License", options.LandingPage.LicenseName);
            Assert.Equal("https://api.com/license.html", options.LandingPage.LicenseUrl.ToString());
            Assert.NotNull(options.LandingPage.Links);
            Assert.NotEmpty(options.LandingPage.Links);
            Assert.Equal(2, options.LandingPage.Links.Count);
            Assert.Equal("https://api.com/landing_page/link1.html", options.LandingPage.Links[0].Href.ToString());
            Assert.Equal("https://api.com/landing_page/link2.html", options.LandingPage.Links[1].Href.ToString());
        }

        [Fact]
        public void ConformanceDeserialization()
        {
            Assert.NotNull(options.Conformance);
            Assert.NotNull(options.Conformance.ConformsTo);
            Assert.NotEmpty(options.Conformance.ConformsTo);
            Assert.Equal(2, options.Conformance.ConformsTo.Count);
            Assert.Equal("https://api.com/conform1.html", options.Conformance.ConformsTo[0].ToString());
            Assert.Equal("https://api.com/conform2.html", options.Conformance.ConformsTo[1].ToString());
        }

        [Fact]
        public void UseApiKeyAuthorizationDeserialization()
        {
            Assert.True(options.UseApiKeyAuthorization);
        }

        [Fact]
        public void CollectionsLinksDeserialization()
        {
            Assert.NotNull(options.Collections);
            Assert.NotNull(options.Collections.Links);
            Assert.NotEmpty(options.Collections.Links);
            Assert.Equal(2, options.Collections.Links.Count);
            Assert.Equal("https://api.com/collections/link1.html", options.Collections.Links[0].Href.ToString());
            Assert.Equal("https://api.com/collections/link2.html", options.Collections.Links[1].Href.ToString());
        }

        [Fact]
        public void CollectionsItemsDeserialization()
        {
            Assert.NotNull(options.Collections);
            Assert.NotNull(options.Collections.Items);
            Assert.NotEmpty(options.Collections.Items);
            Assert.Equal(2, options.Collections.Items.Count);
        }

        [Fact]
        public void FirstCollectionOptionsDeserialization()
        {
            Assert.NotNull(options.Collections.Items[0]);
            Assert.Equal("Collection1", options.Collections.Items[0].Id);
            Assert.Equal("Collection title 1", options.Collections.Items[0].Title);
            Assert.Equal("Collection description 1", options.Collections.Items[0].Description);
            Assert.Equal("Collection1 ItemType", options.Collections.Items[0].ItemType);
            Assert.NotNull(options.Collections.Items[0].Features);
        }

        [Fact]
        public void SecondCollectionOptionsDeserialization()
        {
            Assert.NotNull(options.Collections.Items[1]);
            Assert.Equal("Collection2", options.Collections.Items[1].Id);
            Assert.Equal("Collection title 2", options.Collections.Items[1].Title);
            Assert.Equal("Collection description 2", options.Collections.Items[1].Description);
            Assert.Equal("Collection2 ItemType", options.Collections.Items[1].ItemType);
            Assert.NotNull(options.Collections.Items[1].Features);
        }

        [Fact]
        public void FirstCollectionFeaturesDeserialization()
        {
            Assert.NotNull(options.Collections.Items[0].Features);
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[0].Features.StorageCrs.ToString());
            Assert.Equal("1", options.Collections.Items[0].Features.StorageCrsCoordinateEpoch);
            Assert.NotNull(options.Collections.Items[0].Features.Crs);
            Assert.NotEmpty(options.Collections.Items[0].Features.Crs);
            Assert.Equal(2, options.Collections.Items[0].Features.Crs.Count);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", options.Collections.Items[0].Features.Crs[0].ToString());
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[0].Features.Crs[1].ToString());
            Assert.NotNull(options.Collections.Items[0].Features.Storage);

        }

        [Fact]
        public void SecondCollectionFeaturesDeserialization()
        {
            Assert.NotNull(options.Collections.Items[1].Features);
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[1].Features.StorageCrs.ToString());
            Assert.Equal("2", options.Collections.Items[1].Features.StorageCrsCoordinateEpoch);
            Assert.NotNull(options.Collections.Items[1].Features.Crs);
            Assert.NotEmpty(options.Collections.Items[1].Features.Crs);
            Assert.Equal(2, options.Collections.Items[1].Features.Crs.Count);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", options.Collections.Items[1].Features.Crs[0].ToString());
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[1].Features.Crs[1].ToString());
            Assert.NotNull(options.Collections.Items[1].Features.Storage);

        }

        [Fact]
        public void FirstCollectionStorageDeserialization()
        {
            var storage = options.Collections.Items[0].Features.Storage as SqlCollectionSourceOptions;

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
            var storage = options.Collections.Items[1].Features.Storage as SqlCollectionSourceOptions;

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
            var extent = options.Collections.Items[0].Extent;

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
            var extent = options.Collections.Items[1].Extent;

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
    }

    public class ConfigurationDeserializationFacts
    {
        readonly OgcApiOptions options;
        public ConfigurationDeserializationFacts()
        {
            OptionsUtils.SetupServiceCollection();
            options = OptionsUtils.GetOptionsFromJson();
        }

        [Fact]
        public void OgcApiOptionsDeserialization()
        {
            Assert.NotNull(options);
        }

        [Fact]
        public void LandingPageDeserialization()
        {
            Assert.NotNull(options.LandingPage);
            Assert.Equal("API Title", options.LandingPage.Title);
            Assert.Equal("API Description", options.LandingPage.Description);
            Assert.Equal("1.0", options.LandingPage.Version);
            Assert.Equal("API Contacts", options.LandingPage.ContactName);
            Assert.Equal("https://example.com/", options.LandingPage.ContactUrl.ToString());
            Assert.Equal("https://api.com/index.html", options.LandingPage.ApiDocumentPage.ToString());
            Assert.Equal("https://api.com/swagger.json", options.LandingPage.ApiDescriptionPage.ToString());
            Assert.Equal("API License", options.LandingPage.LicenseName);
            Assert.Equal("https://api.com/license.html", options.LandingPage.LicenseUrl.ToString());
            Assert.NotNull(options.LandingPage.Links);
            Assert.NotEmpty(options.LandingPage.Links);
            Assert.Equal(2, options.LandingPage.Links.Count);
            Assert.Equal("https://api.com/landing_page/link1.html", options.LandingPage.Links[0].Href.ToString());
            Assert.Equal("https://api.com/landing_page/link2.html", options.LandingPage.Links[1].Href.ToString());
        }

        [Fact]
        public void ConformanceDeserialization()
        {
            Assert.NotNull(options.Conformance);
            Assert.NotNull(options.Conformance.ConformsTo);
            Assert.NotEmpty(options.Conformance.ConformsTo);
            Assert.Equal(2, options.Conformance.ConformsTo.Count);
            Assert.Equal("https://api.com/conform1.html", options.Conformance.ConformsTo[0].ToString());
            Assert.Equal("https://api.com/conform2.html", options.Conformance.ConformsTo[1].ToString());
        }

        [Fact]
        public void UseApiKeyAuthorizationDeserialization()
        {
            Assert.True(options.UseApiKeyAuthorization);
        }

        [Fact]
        public void CollectionsLinksDeserialization()
        {
            Assert.NotNull(options.Collections);
            Assert.NotNull(options.Collections.Links);
            Assert.NotEmpty(options.Collections.Links);
            Assert.Equal(2, options.Collections.Links.Count);
            Assert.Equal("https://api.com/collections/link1.html", options.Collections.Links[0].Href.ToString());
            Assert.Equal("https://api.com/collections/link2.html", options.Collections.Links[1].Href.ToString());
        }

        [Fact]
        public void CollectionsItemsDeserialization()
        {
            Assert.NotNull(options.Collections);
            Assert.NotNull(options.Collections.Items);
            Assert.NotEmpty(options.Collections.Items);
            Assert.Equal(2, options.Collections.Items.Count);
        }

        [Fact]
        public void FirstCollectionOptionsDeserialization()
        {
            Assert.NotNull(options.Collections.Items[0]);
            Assert.Equal("Collection1", options.Collections.Items[0].Id);
            Assert.Equal("Collection title 1", options.Collections.Items[0].Title);
            Assert.Equal("Collection description 1", options.Collections.Items[0].Description);
            Assert.Equal("Collection1 ItemType", options.Collections.Items[0].ItemType);
            Assert.NotNull(options.Collections.Items[0].Features);
        }

        [Fact]
        public void SecondCollectionOptionsDeserialization()
        {
            Assert.NotNull(options.Collections.Items[1]);
            Assert.Equal("Collection2", options.Collections.Items[1].Id);
            Assert.Equal("Collection title 2", options.Collections.Items[1].Title);
            Assert.Equal("Collection description 2", options.Collections.Items[1].Description);
            Assert.Equal("Collection2 ItemType", options.Collections.Items[1].ItemType);
            Assert.NotNull(options.Collections.Items[1].Features);
        }

        [Fact]
        public void FirstCollectionFeaturesDeserialization()
        {
            Assert.NotNull(options.Collections.Items[0].Features);
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[0].Features.StorageCrs.ToString());
            Assert.Equal("1", options.Collections.Items[0].Features.StorageCrsCoordinateEpoch);
            Assert.NotNull(options.Collections.Items[0].Features.Crs);
            Assert.NotEmpty(options.Collections.Items[0].Features.Crs);
            Assert.Equal(2, options.Collections.Items[0].Features.Crs.Count);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", options.Collections.Items[0].Features.Crs[0].ToString());
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[0].Features.Crs[1].ToString());
            Assert.NotNull(options.Collections.Items[0].Features.Storage);

        }

        [Fact]
        public void SecondCollectionFeaturesDeserialization()
        {
            Assert.NotNull(options.Collections.Items[1].Features);
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[1].Features.StorageCrs.ToString());
            Assert.Equal("2", options.Collections.Items[1].Features.StorageCrsCoordinateEpoch);
            Assert.NotNull(options.Collections.Items[1].Features.Crs);
            Assert.NotEmpty(options.Collections.Items[1].Features.Crs);
            Assert.Equal(2, options.Collections.Items[1].Features.Crs.Count);
            Assert.Equal("http://www.opengis.net/def/crs/OGC/1.3/CRS84", options.Collections.Items[1].Features.Crs[0].ToString());
            Assert.Equal("http://www.opengis.net/def/crs/EPSG/0/3857", options.Collections.Items[1].Features.Crs[1].ToString());
            Assert.NotNull(options.Collections.Items[1].Features.Storage);

        }

        [Fact]
        public void FirstCollectionStorageDeserialization()
        {
            var storage = options.Collections.Items[0].Features.Storage as SqlCollectionSourceOptions;

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
            var storage = options.Collections.Items[1].Features.Storage as SqlCollectionSourceOptions;

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
            var extent = options.Collections.Items[0].Extent;

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
            var extent = options.Collections.Items[1].Extent;

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
    }
    
    public class ProvidersDeserializationFacts
    {
        readonly CollectionsOptions jsonCollectionsOptions;
        readonly CollectionsOptions sqlServerCollectionsOptions;
        readonly CollectionsOptions postGisCollectionsOptions;
        readonly IDataProvider sqlServerProvider;
        readonly IDataProvider postGisProvider;

        public ProvidersDeserializationFacts()
        {
            OptionsUtils.SetupServiceCollection();
            jsonCollectionsOptions = OptionsUtils.GetOptionsFromJson().Collections;
            sqlServerProvider = OptionsUtils.GetDataProvider("SqlServer");
            postGisProvider = OptionsUtils.GetDataProvider("PostGis");
            sqlServerCollectionsOptions = sqlServerProvider.GetCollectionSourcesOptions() as CollectionsOptions;
            postGisCollectionsOptions = postGisProvider.GetCollectionSourcesOptions() as CollectionsOptions;
        }

        [Fact]
        public void SqlServerProviderOptionsSet()
        {
            Assert.NotNull(sqlServerProvider);
        }

        [Fact]
        public void PostGisProviderOptionsSet()
        {
            Assert.NotNull(postGisProvider);
        }

        [Fact]
        public void SqlServerProviderOptionsLinksSet()
        {
            Assert.NotNull(jsonCollectionsOptions.Links);
            Assert.NotNull(sqlServerCollectionsOptions.Links);
            Assert.Equal(jsonCollectionsOptions.Links.Count, sqlServerCollectionsOptions.Links.Count);
            Assert.Equal(jsonCollectionsOptions.Links[0], sqlServerCollectionsOptions.Links[0]);
            Assert.Equal(jsonCollectionsOptions.Links[1], sqlServerCollectionsOptions.Links[1]);
        }

        [Fact]
        public void PostGisProviderOptionsLinksSet()
        {
            Assert.NotNull(jsonCollectionsOptions.Links);
            Assert.NotNull(postGisCollectionsOptions.Links);
            Assert.NotEmpty(jsonCollectionsOptions.Links);
            Assert.NotEmpty(postGisCollectionsOptions.Links);
            Assert.Equal(jsonCollectionsOptions.Links.Count, postGisCollectionsOptions.Links.Count);
            Assert.Equal(jsonCollectionsOptions.Links[0], postGisCollectionsOptions.Links[0]);
            Assert.Equal(jsonCollectionsOptions.Links[1], postGisCollectionsOptions.Links[1]);
        }

        [Fact]
        public void SqlServerProviderOptionsItemsSet()
        {
            var apiOptions = jsonCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "SqlServer").ToList();

            Assert.NotNull(apiOptions);
            Assert.Single(apiOptions);
            Assert.NotNull(sqlServerCollectionsOptions.Items);
            Assert.Single(sqlServerCollectionsOptions.Items);
            Assert.NotNull(sqlServerCollectionsOptions.Items[0]);

            Assert.Equal(apiOptions[0].Links.Count, sqlServerCollectionsOptions.Items[0].Links.Count);
            Assert.Equal(apiOptions[0].Links[0], sqlServerCollectionsOptions.Items[0].Links[0]);
            Assert.Equal(apiOptions[0].Links[1], sqlServerCollectionsOptions.Items[0].Links[1]);

            Assert.Equal(apiOptions[0].Extent, sqlServerCollectionsOptions.Items[0].Extent);
            Assert.Equal(apiOptions[0].Id, sqlServerCollectionsOptions.Items[0].Id);
            Assert.Equal(apiOptions[0].ItemType, sqlServerCollectionsOptions.Items[0].ItemType);
            Assert.Equal(apiOptions[0].Title, sqlServerCollectionsOptions.Items[0].Title);
            Assert.Equal(apiOptions[0].Description, sqlServerCollectionsOptions.Items[0].Description);
            Assert.Equal(apiOptions[0].Features.StorageCrs, sqlServerCollectionsOptions.Items[0].Features.StorageCrs);

            Assert.Equal(apiOptions[0].Features.Crs.Count, sqlServerCollectionsOptions.Items[0].Features.Crs.Count);
            Assert.Equal(apiOptions[0].Features.Crs[0], sqlServerCollectionsOptions.Items[0].Features.Crs[0]);
            Assert.Equal(apiOptions[0].Features.Crs[1], sqlServerCollectionsOptions.Items[0].Features.Crs[1]);
        }

        [Fact]
        public void PostGisProviderOptionsItemsSet()
        {
            var apiOptions = jsonCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "PostGis").ToList();

            Assert.NotNull(apiOptions);
            Assert.Single(apiOptions);
            Assert.NotNull(postGisCollectionsOptions.Items);
            Assert.Single(postGisCollectionsOptions.Items);
            Assert.NotNull(postGisCollectionsOptions.Items[0]);

            Assert.Equal(apiOptions[0].Links.Count, postGisCollectionsOptions.Items[0].Links.Count);
            Assert.Equal(apiOptions[0].Links[0], postGisCollectionsOptions.Items[0].Links[0]);
            Assert.Equal(apiOptions[0].Links[1], postGisCollectionsOptions.Items[0].Links[1]);

            Assert.Equal(apiOptions[0].Extent, postGisCollectionsOptions.Items[0].Extent);
            Assert.Equal(apiOptions[0].Id, postGisCollectionsOptions.Items[0].Id);
            Assert.Equal(apiOptions[0].ItemType, postGisCollectionsOptions.Items[0].ItemType);
            Assert.Equal(apiOptions[0].Title, postGisCollectionsOptions.Items[0].Title);
            Assert.Equal(apiOptions[0].Description, postGisCollectionsOptions.Items[0].Description);
            Assert.Equal(apiOptions[0].Features.StorageCrs, postGisCollectionsOptions.Items[0].Features.StorageCrs);

            Assert.Equal(apiOptions[0].Features.Crs.Count, postGisCollectionsOptions.Items[0].Features.Crs.Count);
            Assert.Equal(apiOptions[0].Features.Crs[0], postGisCollectionsOptions.Items[0].Features.Crs[0]);
            Assert.Equal(apiOptions[0].Features.Crs[1], postGisCollectionsOptions.Items[0].Features.Crs[1]);
        }

        [Fact]
        public void SqlServerProviderOptionsStorageSet()
        {
            var providerStorage = sqlServerCollectionsOptions.Items[0].Features.Storage as SqlCollectionSourceOptions;
            var apiStorage = jsonCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "SqlServer").ToList()[0].Features.Storage as SqlCollectionSourceOptions;

            Assert.NotNull(apiStorage);
            Assert.NotNull(providerStorage);
            Assert.Equal(apiStorage.Type, providerStorage.Type);
            Assert.Equal(apiStorage.Table, providerStorage.Table);
            Assert.Equal(apiStorage.Schema, providerStorage.Schema);
            Assert.Equal(apiStorage.IdentifierColumn, providerStorage.IdentifierColumn);
            Assert.Equal(apiStorage.GeometrySrid, providerStorage.GeometrySrid);
            Assert.Equal(apiStorage.GeometryGeoJsonType, providerStorage.GeometryGeoJsonType);
            Assert.Equal(apiStorage.GeometryDataType, providerStorage.GeometryDataType);
            Assert.Equal(apiStorage.GeometryColumn, providerStorage.GeometryColumn);
            Assert.Equal(apiStorage.DateTimeColumn, providerStorage.DateTimeColumn);
            Assert.Equal(apiStorage.ConnectionString, providerStorage.ConnectionString);
            Assert.Equal(apiStorage.ApiKeyPredicateForUpdate, providerStorage.ApiKeyPredicateForUpdate);
            Assert.Equal(apiStorage.ApiKeyPredicateForGet, providerStorage.ApiKeyPredicateForGet);
            Assert.Equal(apiStorage.ApiKeyPredicateForDelete, providerStorage.ApiKeyPredicateForDelete);
            Assert.Equal(apiStorage.ApiKeyPredicateForCreate, providerStorage.ApiKeyPredicateForCreate);
            Assert.Equal(apiStorage.AllowUpdate, providerStorage.AllowUpdate);
            Assert.Equal(apiStorage.AllowReplace, providerStorage.AllowReplace);
            Assert.Equal(apiStorage.AllowDelete, providerStorage.AllowDelete);
            Assert.Equal(apiStorage.AllowCreate, providerStorage.AllowCreate);

            Assert.NotNull(apiStorage.Properties);
            Assert.NotEmpty(apiStorage.Properties);
            Assert.NotNull(providerStorage.Properties);
            Assert.NotEmpty(providerStorage.Properties);
            Assert.Equal(apiStorage.Properties.Count, providerStorage.Properties.Count);
            Assert.Equal(apiStorage.Properties[0], providerStorage.Properties[0]);
            Assert.Equal(apiStorage.Properties[1], providerStorage.Properties[1]);
        }

        [Fact]
        public void PostGisProviderOptionsStorageSet()
        {
            var providerStorage = jsonCollectionsOptions.Items[0].Features.Storage as SqlCollectionSourceOptions;
            var apiStorage = postGisCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "PostGis").ToList()[0].Features.Storage as SqlCollectionSourceOptions;

            Assert.NotNull(apiStorage);
            Assert.NotNull(providerStorage);
            Assert.Equal(apiStorage.Type, providerStorage.Type);
            Assert.Equal(apiStorage.Table, providerStorage.Table);
            Assert.Equal(apiStorage.Schema, providerStorage.Schema);
            Assert.Equal(apiStorage.IdentifierColumn, providerStorage.IdentifierColumn);
            Assert.Equal(apiStorage.GeometrySrid, providerStorage.GeometrySrid);
            Assert.Equal(apiStorage.GeometryGeoJsonType, providerStorage.GeometryGeoJsonType);
            Assert.Equal(apiStorage.GeometryDataType, providerStorage.GeometryDataType);
            Assert.Equal(apiStorage.GeometryColumn, providerStorage.GeometryColumn);
            Assert.Equal(apiStorage.DateTimeColumn, providerStorage.DateTimeColumn);
            Assert.Equal(apiStorage.ConnectionString, providerStorage.ConnectionString);
            Assert.Equal(apiStorage.ApiKeyPredicateForUpdate, providerStorage.ApiKeyPredicateForUpdate);
            Assert.Equal(apiStorage.ApiKeyPredicateForGet, providerStorage.ApiKeyPredicateForGet);
            Assert.Equal(apiStorage.ApiKeyPredicateForDelete, providerStorage.ApiKeyPredicateForDelete);
            Assert.Equal(apiStorage.ApiKeyPredicateForCreate, providerStorage.ApiKeyPredicateForCreate);
            Assert.Equal(apiStorage.AllowUpdate, providerStorage.AllowUpdate);
            Assert.Equal(apiStorage.AllowReplace, providerStorage.AllowReplace);
            Assert.Equal(apiStorage.AllowDelete, providerStorage.AllowDelete);
            Assert.Equal(apiStorage.AllowCreate, providerStorage.AllowCreate);

            Assert.NotNull(apiStorage.Properties);
            Assert.NotEmpty(apiStorage.Properties);
            Assert.NotNull(providerStorage.Properties);
            Assert.NotEmpty(providerStorage.Properties);
            Assert.Equal(apiStorage.Properties.Count, providerStorage.Properties.Count);
            Assert.Equal(apiStorage.Properties[0], providerStorage.Properties[0]);
            Assert.Equal(apiStorage.Properties[1], providerStorage.Properties[1]);
        }
    }
}


