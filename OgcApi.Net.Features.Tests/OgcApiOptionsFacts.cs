using OgcApi.Net.Features.Tests.Utils;
using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using System.Linq;
using Xunit;

namespace OgcApi.Net.Features.Tests
{
    public class OgcApiOptionsFacts
    {
        [Fact]
        public void SettingsSerializationExecuted()
        {
            var res = OptionsUtils.SerializeOgcApiOptions(OptionsUtils.GetOptionsFromCode());

            Assert.False(string.IsNullOrEmpty(res));
        }

        [Fact]
        public void SettingsDeserializationExecuted()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options);
        }

        [Fact]
        public void LandingPageDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

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
        public void ConformanceDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.Conformance);
            Assert.NotNull(options.Conformance.ConformsTo);
            Assert.NotEmpty(options.Conformance.ConformsTo);
            Assert.Equal(2, options.Conformance.ConformsTo.Count);
            Assert.Equal("https://api.com/conform1.html", options.Conformance.ConformsTo[0].ToString());
            Assert.Equal("https://api.com/conform2.html", options.Conformance.ConformsTo[1].ToString());
        }

        [Fact]
        public void UseApiKeyAuthorizationDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.True(options.UseApiKeyAuthorization);
        }

        [Fact]
        public void CollectionsLinksDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.Collections);
            Assert.NotNull(options.Collections.Links);
            Assert.NotEmpty(options.Collections.Links);
            Assert.Equal(2, options.Collections.Links.Count);
            Assert.Equal("https://api.com/collections/link1.html", options.Collections.Links[0].Href.ToString());
            Assert.Equal("https://api.com/collections/link2.html", options.Collections.Links[1].Href.ToString());
        }

        [Fact]
        public void CollectionsItemsDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.Collections);
            Assert.NotNull(options.Collections.Items);
            Assert.NotEmpty(options.Collections.Items);
            Assert.Equal(2, options.Collections.Items.Count);
        }

        [Fact]
        public void FirstCollectionOptionsDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.Collections.Items[0]);
            Assert.Equal("Collection1", options.Collections.Items[0].Id);
            Assert.Equal("Collection title 1", options.Collections.Items[0].Title);
            Assert.Equal("Collection description 1", options.Collections.Items[0].Description);
            Assert.Equal("Collection1 ItemType", options.Collections.Items[0].ItemType);
            Assert.NotNull(options.Collections.Items[0].Features);
        }

        [Fact]
        public void SecondCollectionOptionsDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

            Assert.NotNull(options.Collections.Items[1]);
            Assert.Equal("Collection2", options.Collections.Items[1].Id);
            Assert.Equal("Collection title 2", options.Collections.Items[1].Title);
            Assert.Equal("Collection description 2", options.Collections.Items[1].Description);
            Assert.Equal("Collection2 ItemType", options.Collections.Items[1].ItemType);
            Assert.NotNull(options.Collections.Items[1].Features);
        }

        [Fact]
        public void FirstCollectionFeaturesOptionsDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

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
        public void SecondCollectionFeaturesOptionsDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();

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
        public void FirstCollectionFeaturesOptionsStorageDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();
            var storage = options.Collections.Items[0].Features.Storage as SqlFeaturesSourceOptions;
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
        public void SecondCollectionFeaturesOptionsStorageDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();
            var storage = options.Collections.Items[1].Features.Storage as SqlFeaturesSourceOptions;
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
        public void FirstCollectionOptionsExtentDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();
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
        public void SecondCollectionOptionsExtentDeserializationValid()
        {
            var options = OptionsUtils.GetOptionsFromJson();
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

        [Fact]
        public void SqlServerProviderOptionsSet()
        {
            _ = OptionsUtils.GetOptionsFromJson();
            var provider = OptionsUtils.GetDataProvider("SqlServer");

            Assert.NotNull(provider);
        }

        [Fact]
        public void PostGisProviderOptionsSet()
        {
            _ = OptionsUtils.GetOptionsFromJson();
            var provider = OptionsUtils.GetDataProvider("PostGis");

            Assert.NotNull(provider);
        }

        [Fact]
        public void SqlServerProviderOptionsLinksSet()
        {
            var options = OptionsUtils.GetOptionsFromJson().Collections;
            var provider = OptionsUtils.GetDataProvider("SqlServer");
            var providerOptions = provider.CollectionsOptions as CollectionsOptions;

            Assert.NotNull(options.Links);
            Assert.NotNull(providerOptions.Links);
            Assert.Equal(options.Links.Count, providerOptions.Links.Count);
            Assert.Equal(options.Links[0], providerOptions.Links[0]);
            Assert.Equal(options.Links[1], providerOptions.Links[1]);
        }

        [Fact]
        public void PostGisProviderOptionsLinksSet()
        {
            var options = OptionsUtils.GetOptionsFromJson().Collections;
            var provider = OptionsUtils.GetDataProvider("PostGis");
            var providerOptions = provider.CollectionsOptions as CollectionsOptions;

            Assert.NotNull(options.Links);
            Assert.NotNull(providerOptions.Links);
            Assert.NotEmpty(options.Links);
            Assert.NotEmpty(providerOptions.Links);
            Assert.Equal(options.Links.Count, providerOptions.Links.Count);
            Assert.Equal(options.Links[0], providerOptions.Links[0]);
            Assert.Equal(options.Links[1], providerOptions.Links[1]);
        }

        [Fact]
        public void SqlServerProviderOptionsItemsSet()
        {
            var apiOptions = OptionsUtils.GetOptionsFromJson().Collections.Items.Where(i => i.Features.Storage.Type == "SqlServer").ToList();
            var provider = OptionsUtils.GetDataProvider("SqlServer");
            var providerOptions = provider.CollectionsOptions as CollectionsOptions;

            Assert.NotNull(apiOptions);
            Assert.Single(apiOptions);
            Assert.NotNull(providerOptions.Items);
            Assert.Single(providerOptions.Items);
            Assert.NotNull(providerOptions.Items[0]);

            Assert.Equal(apiOptions[0].Links.Count, providerOptions.Items[0].Links.Count);
            Assert.Equal(apiOptions[0].Links[0], providerOptions.Items[0].Links[0]);
            Assert.Equal(apiOptions[0].Links[1], providerOptions.Items[0].Links[1]);

            Assert.Equal(apiOptions[0].Extent, providerOptions.Items[0].Extent);
            Assert.Equal(apiOptions[0].Id, providerOptions.Items[0].Id);
            Assert.Equal(apiOptions[0].ItemType, providerOptions.Items[0].ItemType);
            Assert.Equal(apiOptions[0].Title, providerOptions.Items[0].Title);
            Assert.Equal(apiOptions[0].Description, providerOptions.Items[0].Description);
            Assert.Equal(apiOptions[0].Features.StorageCrs, providerOptions.Items[0].Features.StorageCrs);

            Assert.Equal(apiOptions[0].Features.Crs.Count, providerOptions.Items[0].Features.Crs.Count);
            Assert.Equal(apiOptions[0].Features.Crs[0], providerOptions.Items[0].Features.Crs[0]);
            Assert.Equal(apiOptions[0].Features.Crs[1], providerOptions.Items[0].Features.Crs[1]);
        }

        [Fact]
        public void PostGisProviderOptionsItemsSet()
        {
            var apiOptions = OptionsUtils.GetOptionsFromJson().Collections.Items.Where(i => i.Features.Storage.Type == "PostGis").ToList();
            var provider = OptionsUtils.GetDataProvider("PostGis");
            var providerOptions = provider.CollectionsOptions as CollectionsOptions;

            Assert.NotNull(apiOptions);
            Assert.Single(apiOptions);
            Assert.NotNull(providerOptions.Items);
            Assert.Single(providerOptions.Items);
            Assert.NotNull(providerOptions.Items[0]);

            Assert.Equal(apiOptions[0].Links.Count, providerOptions.Items[0].Links.Count);
            Assert.Equal(apiOptions[0].Links[0], providerOptions.Items[0].Links[0]);
            Assert.Equal(apiOptions[0].Links[1], providerOptions.Items[0].Links[1]);

            Assert.Equal(apiOptions[0].Extent, providerOptions.Items[0].Extent);
            Assert.Equal(apiOptions[0].Id, providerOptions.Items[0].Id);
            Assert.Equal(apiOptions[0].ItemType, providerOptions.Items[0].ItemType);
            Assert.Equal(apiOptions[0].Title, providerOptions.Items[0].Title);
            Assert.Equal(apiOptions[0].Description, providerOptions.Items[0].Description);
            Assert.Equal(apiOptions[0].Features.StorageCrs, providerOptions.Items[0].Features.StorageCrs);

            Assert.Equal(apiOptions[0].Features.Crs.Count, providerOptions.Items[0].Features.Crs.Count);
            Assert.Equal(apiOptions[0].Features.Crs[0], providerOptions.Items[0].Features.Crs[0]);
            Assert.Equal(apiOptions[0].Features.Crs[1], providerOptions.Items[0].Features.Crs[1]);
        }

        [Fact]
        public void SqlServerProviderOptionsStorageSet()
        {
            var provider = OptionsUtils.GetDataProvider("SqlServer");
            var providerStorage = (provider.CollectionsOptions as CollectionsOptions).Items[0].Features.Storage as SqlFeaturesSourceOptions;
            var apiStorage = (OptionsUtils.GetOptionsFromJson().Collections.Items.Where(i => i.Features.Storage.Type == "SqlServer").ToList())[0].Features.Storage as SqlFeaturesSourceOptions;

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
            var provider = OptionsUtils.GetDataProvider("PostGis");
            var providerStorage = (provider.CollectionsOptions as CollectionsOptions)?.Items[0].Features.Storage as SqlFeaturesSourceOptions;
            var apiStorage = OptionsUtils.GetOptionsFromJson().Collections.Items.Where(i => i.Features.Storage.Type == "PostGis").ToList()[0].Features.Storage as SqlFeaturesSourceOptions;

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


