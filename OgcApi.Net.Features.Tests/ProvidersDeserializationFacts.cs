using OgcApi.Net.Features.Options.Features;
using System.Linq;
using Xunit;

namespace OgcApi.Net.Features.Tests
{
    public class ProvidersDeserializationFacts : IClassFixture<ProvidersOptionsFixture>
    {
        private readonly ProvidersOptionsFixture _fixture;
        public ProvidersDeserializationFacts(ProvidersOptionsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void SqlServerProviderOptionsSet()
        {
            Assert.NotNull(_fixture.sqlServerProvider);
            Assert.NotNull(_fixture.sqlServerCollectionsOptions);
        }

        [Fact]
        public void PostGisProviderOptionsSet()
        {
            Assert.NotNull(_fixture.postGisProvider);
            Assert.NotNull(_fixture.postGisCollectionsOptions);
        }

        [Fact]
        public void SqlServerProviderOptionsLinksSet()
        {
            Assert.NotNull(_fixture.jsonCollectionsOptions.Links);
            Assert.NotNull(_fixture.sqlServerCollectionsOptions.Links);
            Assert.Equal(_fixture.jsonCollectionsOptions.Links.Count, _fixture.sqlServerCollectionsOptions.Links.Count);
            Assert.Equal(_fixture.jsonCollectionsOptions.Links[0].Href, _fixture.sqlServerCollectionsOptions.Links[0].Href);
            Assert.Equal(_fixture.jsonCollectionsOptions.Links[1].Href, _fixture.sqlServerCollectionsOptions.Links[1].Href);
        }

        [Fact]
        public void PostGisProviderOptionsLinksSet()
        {
            Assert.NotNull(_fixture.jsonCollectionsOptions.Links);
            Assert.NotNull(_fixture.postGisCollectionsOptions.Links);
            Assert.NotEmpty(_fixture.jsonCollectionsOptions.Links);
            Assert.NotEmpty(_fixture.postGisCollectionsOptions.Links);
            Assert.Equal(_fixture.jsonCollectionsOptions.Links.Count, _fixture.postGisCollectionsOptions.Links.Count);
            Assert.Equal(_fixture.jsonCollectionsOptions.Links[0].Href, _fixture.postGisCollectionsOptions.Links[0].Href);
            Assert.Equal(_fixture.jsonCollectionsOptions.Links[1].Href, _fixture.postGisCollectionsOptions.Links[1].Href);
        }

        [Fact]
        public void SqlServerProviderOptionsItemsSet()
        {
            var apiOptions = _fixture.jsonCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "SqlServer").ToList();

            Assert.NotNull(apiOptions);
            Assert.Single(apiOptions);
            Assert.NotNull(_fixture.sqlServerCollectionsOptions.Items);
            Assert.Single(_fixture.sqlServerCollectionsOptions.Items);
            Assert.NotNull(_fixture.sqlServerCollectionsOptions.Items[0]);

            Assert.Equal(apiOptions[0].Links.Count, _fixture.sqlServerCollectionsOptions.Items[0].Links.Count);
            Assert.Equal(apiOptions[0].Links[0].Href, _fixture.sqlServerCollectionsOptions.Items[0].Links[0].Href);
            Assert.Equal(apiOptions[0].Links[1].Href, _fixture.sqlServerCollectionsOptions.Items[0].Links[1].Href);

            Assert.Equal(apiOptions[0].Extent.Spatial.Crs, _fixture.sqlServerCollectionsOptions.Items[0].Extent.Spatial.Crs);
            Assert.Equal(apiOptions[0].Extent.Spatial.Bbox.Length, _fixture.sqlServerCollectionsOptions.Items[0].Extent.Spatial.Bbox.Length);
            Assert.Equal(apiOptions[0].Extent.Temporal.Trs, _fixture.sqlServerCollectionsOptions.Items[0].Extent.Temporal.Trs);
            Assert.Equal(apiOptions[0].Extent.Temporal.Interval.Length, _fixture.sqlServerCollectionsOptions.Items[0].Extent.Temporal.Interval.Length);
            Assert.Equal(apiOptions[0].Id, _fixture.sqlServerCollectionsOptions.Items[0].Id);
            Assert.Equal(apiOptions[0].ItemType, _fixture.sqlServerCollectionsOptions.Items[0].ItemType);
            Assert.Equal(apiOptions[0].Title, _fixture.sqlServerCollectionsOptions.Items[0].Title);
            Assert.Equal(apiOptions[0].Description, _fixture.sqlServerCollectionsOptions.Items[0].Description);
            Assert.Equal(apiOptions[0].Features.StorageCrs, _fixture.sqlServerCollectionsOptions.Items[0].Features.StorageCrs);

            Assert.Equal(apiOptions[0].Features.Crs.Count, _fixture.sqlServerCollectionsOptions.Items[0].Features.Crs.Count);
            Assert.Equal(apiOptions[0].Features.Crs[0], _fixture.sqlServerCollectionsOptions.Items[0].Features.Crs[0]);
            Assert.Equal(apiOptions[0].Features.Crs[1], _fixture.sqlServerCollectionsOptions.Items[0].Features.Crs[1]);
        }

        [Fact]
        public void PostGisProviderOptionsItemsSet()
        {
            var apiOptions = _fixture.jsonCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "PostGis").ToList();

            Assert.NotNull(apiOptions);
            Assert.Single(apiOptions);
            Assert.NotNull(_fixture.postGisCollectionsOptions.Items);
            Assert.Single(_fixture.postGisCollectionsOptions.Items);
            Assert.NotNull(_fixture.postGisCollectionsOptions.Items[0]);

            Assert.Equal(apiOptions[0].Links.Count, _fixture.postGisCollectionsOptions.Items[0].Links.Count);
            Assert.Equal(apiOptions[0].Links[0].Href, _fixture.postGisCollectionsOptions.Items[0].Links[0].Href);
            Assert.Equal(apiOptions[0].Links[1].Href, _fixture.postGisCollectionsOptions.Items[0].Links[1].Href);

            Assert.Equal(apiOptions[0].Extent.Spatial.Crs, _fixture.postGisCollectionsOptions.Items[0].Extent.Spatial.Crs);
            Assert.Equal(apiOptions[0].Extent.Spatial.Bbox.Length, _fixture.postGisCollectionsOptions.Items[0].Extent.Spatial.Bbox.Length);
            Assert.Equal(apiOptions[0].Extent.Temporal.Trs, _fixture.postGisCollectionsOptions.Items[0].Extent.Temporal.Trs);
            Assert.Equal(apiOptions[0].Extent.Temporal.Interval.Length, _fixture.postGisCollectionsOptions.Items[0].Extent.Temporal.Interval.Length);

            Assert.Equal(apiOptions[0].Id, _fixture.postGisCollectionsOptions.Items[0].Id);
            Assert.Equal(apiOptions[0].ItemType, _fixture.postGisCollectionsOptions.Items[0].ItemType);
            Assert.Equal(apiOptions[0].Title, _fixture.postGisCollectionsOptions.Items[0].Title);
            Assert.Equal(apiOptions[0].Description, _fixture.postGisCollectionsOptions.Items[0].Description);
            Assert.Equal(apiOptions[0].Features.StorageCrs, _fixture.postGisCollectionsOptions.Items[0].Features.StorageCrs);

            Assert.Equal(apiOptions[0].Features.Crs.Count, _fixture.postGisCollectionsOptions.Items[0].Features.Crs.Count);
            Assert.Equal(apiOptions[0].Features.Crs[0], _fixture.postGisCollectionsOptions.Items[0].Features.Crs[0]);
            Assert.Equal(apiOptions[0].Features.Crs[1], _fixture.postGisCollectionsOptions.Items[0].Features.Crs[1]);
        }

        [Fact]
        public void SqlServerProviderOptionsStorageSet()
        {
            var providerStorage = _fixture.sqlServerCollectionsOptions.Items[0].Features.Storage as SqlFeaturesSourceOptions;
            var apiStorage = _fixture.jsonCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "SqlServer").ToList()[0].Features.Storage as SqlFeaturesSourceOptions;

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
            var providerStorage = _fixture.jsonCollectionsOptions.Items[0].Features.Storage as SqlFeaturesSourceOptions;
            var apiStorage = _fixture.postGisCollectionsOptions.Items.Where(i => i.Features.Storage.Type == "PostGis").ToList()[0].Features.Storage as SqlFeaturesSourceOptions;

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


