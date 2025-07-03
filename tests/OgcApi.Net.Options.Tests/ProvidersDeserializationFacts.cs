using OgcApi.Net.Options;
using OgcApi.Net.Options.Features;
using OgcApi.Options.Tests.Utils;
using System.Linq;
using Xunit;

namespace OgcApi.Options.Tests;

public class ProvidersDeserializationFacts(ConfigurationOptionsFixture fixture)
    : IClassFixture<ConfigurationOptionsFixture>
{
    private readonly OgcApiOptions _options = fixture.Options;

    [Fact]
    public void SqlServerProviderOptionsLinksSet()
    {
        var expectedApiOptions = OptionsUtils.GetOptionsFromCode();
        var expectedCollectionOptions =
            expectedApiOptions.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "SqlServer");
        var actualCollectionOptions =
            _options.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "SqlServer");

        Assert.NotNull(expectedCollectionOptions);
        Assert.NotNull(actualCollectionOptions);

        Assert.Equal(expectedCollectionOptions.Links.Count, actualCollectionOptions.Links.Count);
        Assert.Equal(expectedCollectionOptions.Links[0].Href, actualCollectionOptions.Links[0].Href);
        Assert.Equal(expectedCollectionOptions.Links[1].Href, actualCollectionOptions.Links[1].Href);
    }

    [Fact]
    public void PostGisProviderOptionsLinksSet()
    {
        var expectedApiOptions = OptionsUtils.GetOptionsFromCode();
        var expectedCollectionOptions =
            expectedApiOptions.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "PostGis");
        var actualCollectionOptions =
            _options.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "PostGis");

        Assert.NotNull(expectedCollectionOptions);
        Assert.NotNull(actualCollectionOptions);

        Assert.Equal(expectedCollectionOptions.Links.Count, actualCollectionOptions.Links.Count);
        Assert.Equal(expectedCollectionOptions.Links[0].Href, actualCollectionOptions.Links[0].Href);
        Assert.Equal(expectedCollectionOptions.Links[1].Href, actualCollectionOptions.Links[1].Href);
    }

    [Fact]
    public void SqlServerProviderOptionsItemsSet()
    {
        var expectedApiOptions = OptionsUtils.GetOptionsFromCode();
        var expectedCollectionOptions =
            expectedApiOptions.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "SqlServer");
        var actualCollectionOptions =
            _options.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "SqlServer");

        Assert.NotNull(expectedCollectionOptions);
        Assert.NotNull(actualCollectionOptions);

        Assert.Equal(expectedCollectionOptions.Id, actualCollectionOptions.Id);
        Assert.Equal(expectedCollectionOptions.Title, actualCollectionOptions.Title);
        Assert.Equal(expectedCollectionOptions.Description, actualCollectionOptions.Description);
        Assert.Equal(expectedCollectionOptions.Features.StorageCrs, actualCollectionOptions.Features.StorageCrs);
        Assert.Equal(expectedCollectionOptions.Features.Crs.Count, actualCollectionOptions.Features.Crs.Count);
        Assert.Equal(expectedCollectionOptions.Features.Crs[0], actualCollectionOptions.Features.Crs[0]);
        Assert.Equal(expectedCollectionOptions.Features.Crs[1], actualCollectionOptions.Features.Crs[1]);
    }

    [Fact]
    public void PostGisProviderOptionsItemsSet()
    {
        var expectedApiOptions = OptionsUtils.GetOptionsFromCode();
        var expectedCollectionOptions =
            expectedApiOptions.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "PostGis");
        var actualCollectionOptions =
            _options.Collections.Items.FirstOrDefault(collection => collection.Features.Storage.Type == "PostGis");

        Assert.NotNull(expectedCollectionOptions);
        Assert.NotNull(actualCollectionOptions);

        Assert.Equal(expectedCollectionOptions.Extent.Spatial.Crs, actualCollectionOptions.Extent.Spatial.Crs);
        Assert.Equal(expectedCollectionOptions.Extent.Spatial.Bbox.Length, actualCollectionOptions.Extent.Spatial.Bbox.Length);
        Assert.Equal(expectedCollectionOptions.Extent.Temporal.Trs, actualCollectionOptions.Extent.Temporal.Trs);
        Assert.Equal(expectedCollectionOptions.Extent.Temporal.Interval.Length, actualCollectionOptions.Extent.Temporal.Interval.Length);
        Assert.Equal(expectedCollectionOptions.Id, actualCollectionOptions.Id);
        Assert.Equal(expectedCollectionOptions.Title, actualCollectionOptions.Title);
        Assert.Equal(expectedCollectionOptions.Description, actualCollectionOptions.Description);
        Assert.Equal(expectedCollectionOptions.Features.StorageCrs, actualCollectionOptions.Features.StorageCrs);
        Assert.Equal(expectedCollectionOptions.Features.Crs.Count, actualCollectionOptions.Features.Crs.Count);
        Assert.Equal(expectedCollectionOptions.Features.Crs[0], actualCollectionOptions.Features.Crs[0]);
        Assert.Equal(expectedCollectionOptions.Features.Crs[1], actualCollectionOptions.Features.Crs[1]);
    }

    [Fact]
    public void SqlServerProviderOptionsStorageSet()
    {
        var expectedApiOptions = OptionsUtils.GetOptionsFromCode();
        var expectedCollectionOptions =
            expectedApiOptions.Collections.Items.First(collection => collection.Features.Storage.Type == "SqlServer").Features.Storage as SqlFeaturesSourceOptions;
        var actualCollectionOptions =
            _options.Collections.Items.First(collection => collection.Features.Storage.Type == "SqlServer").Features.Storage as SqlFeaturesSourceOptions;

        Assert.NotNull(expectedCollectionOptions);
        Assert.NotNull(actualCollectionOptions);

        Assert.Equal(expectedCollectionOptions.Type, actualCollectionOptions.Type);
        Assert.Equal(expectedCollectionOptions.Table, actualCollectionOptions.Table);
        Assert.Equal(expectedCollectionOptions.Schema, actualCollectionOptions.Schema);
        Assert.Equal(expectedCollectionOptions.IdentifierColumn, actualCollectionOptions.IdentifierColumn);
        Assert.Equal(expectedCollectionOptions.GeometrySrid, actualCollectionOptions.GeometrySrid);
        Assert.Equal(expectedCollectionOptions.GeometryGeoJsonType, actualCollectionOptions.GeometryGeoJsonType);
        Assert.Equal(expectedCollectionOptions.GeometryDataType, actualCollectionOptions.GeometryDataType);
        Assert.Equal(expectedCollectionOptions.GeometryColumn, actualCollectionOptions.GeometryColumn);
        Assert.Equal(expectedCollectionOptions.ConnectionString, actualCollectionOptions.ConnectionString);
        Assert.Equal(expectedCollectionOptions.ApiKeyPredicateForUpdate, actualCollectionOptions.ApiKeyPredicateForUpdate);
        Assert.Equal(expectedCollectionOptions.ApiKeyPredicateForGet, actualCollectionOptions.ApiKeyPredicateForGet);
        Assert.Equal(expectedCollectionOptions.ApiKeyPredicateForCreate, actualCollectionOptions.ApiKeyPredicateForCreate);
        Assert.Equal(expectedCollectionOptions.AllowUpdate, actualCollectionOptions.AllowUpdate);
        Assert.Equal(expectedCollectionOptions.AllowReplace, actualCollectionOptions.AllowReplace);
        Assert.Equal(expectedCollectionOptions.AllowCreate, actualCollectionOptions.AllowCreate);

        Assert.NotNull(expectedCollectionOptions.Properties);
        Assert.NotEmpty(expectedCollectionOptions.Properties);
        Assert.NotNull(actualCollectionOptions.Properties);
        Assert.NotEmpty(actualCollectionOptions.Properties);
        Assert.Equal(expectedCollectionOptions.Properties.Count, actualCollectionOptions.Properties.Count);
        Assert.Equal(expectedCollectionOptions.Properties[0], actualCollectionOptions.Properties[0]);
        Assert.Equal(expectedCollectionOptions.Properties[1], actualCollectionOptions.Properties[1]);

    }

    [Fact]
    public void PostGisProviderOptionsStorageSet()
    {
        var expectedApiOptions = OptionsUtils.GetOptionsFromCode();
        var expectedCollectionOptions =
            expectedApiOptions.Collections.Items.First(collection => collection.Features.Storage.Type == "PostGis").Features.Storage as SqlFeaturesSourceOptions;
        var actualCollectionOptions =
            _options.Collections.Items.First(collection => collection.Features.Storage.Type == "PostGis").Features.Storage as SqlFeaturesSourceOptions;

        Assert.NotNull(expectedCollectionOptions);
        Assert.NotNull(actualCollectionOptions);

        Assert.Equal(expectedCollectionOptions.Type, actualCollectionOptions.Type);
        Assert.Equal(expectedCollectionOptions.Table, actualCollectionOptions.Table);
        Assert.Equal(expectedCollectionOptions.Schema, actualCollectionOptions.Schema);
        Assert.Equal(expectedCollectionOptions.IdentifierColumn, actualCollectionOptions.IdentifierColumn);
        Assert.Equal(expectedCollectionOptions.GeometrySrid, actualCollectionOptions.GeometrySrid);
        Assert.Equal(expectedCollectionOptions.GeometryGeoJsonType, actualCollectionOptions.GeometryGeoJsonType);
        Assert.Equal(expectedCollectionOptions.GeometryDataType, actualCollectionOptions.GeometryDataType);
        Assert.Equal(expectedCollectionOptions.ConnectionString, actualCollectionOptions.ConnectionString);
        Assert.Equal(expectedCollectionOptions.ApiKeyPredicateForUpdate, actualCollectionOptions.ApiKeyPredicateForUpdate);
        Assert.Equal(expectedCollectionOptions.ApiKeyPredicateForGet, actualCollectionOptions.ApiKeyPredicateForGet);
        Assert.Equal(expectedCollectionOptions.ApiKeyPredicateForCreate, actualCollectionOptions.ApiKeyPredicateForCreate);
        Assert.Equal(expectedCollectionOptions.AllowUpdate, actualCollectionOptions.AllowUpdate);
        Assert.Equal(expectedCollectionOptions.AllowReplace, actualCollectionOptions.AllowReplace);
        Assert.Equal(expectedCollectionOptions.AllowCreate, actualCollectionOptions.AllowCreate);

        Assert.NotNull(expectedCollectionOptions.Properties);
        Assert.NotEmpty(expectedCollectionOptions.Properties);
        Assert.NotNull(actualCollectionOptions.Properties);
        Assert.NotEmpty(actualCollectionOptions.Properties);
        Assert.Equal(expectedCollectionOptions.Properties.Count, actualCollectionOptions.Properties.Count);
    }
}