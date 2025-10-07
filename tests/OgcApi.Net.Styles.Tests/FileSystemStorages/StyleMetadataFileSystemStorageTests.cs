using OgcApi.Net.Styles.Model.Metadata;
using OgcApi.Net.Styles.Storage.FileSystem;
using OgcApi.Net.Styles.Tests.Mocks;
using System.Text.Json;

namespace OgcApi.Net.Styles.Tests.FileSystemStorages;

[Collection("FileSystemStorageTests")]
public class StyleMetadataFileSystemStorageTests
{
    private readonly StyleMetadataFileSystemStorage _metadataStorage;
    private readonly StyleFileSystemStorageOptions _options;

    public StyleMetadataFileSystemStorageTests()
    {
        var optionsMonitor = OptionsMonitorMock.Instance;
        _options = optionsMonitor.CurrentValue;
        _metadataStorage = new StyleMetadataFileSystemStorage(optionsMonitor);

        FileSystemFixture.CreateInitialStyle(_options);
    }

    [Fact]
    public async Task AddMetadata_ShouldAddMetadata()
    {
        const string collectionId = FileSystemFixture.CollectionId;
        const string styleId = FileSystemFixture.ExistingStyleId;
        const string expectedTitle = "newlyAddedMetadata";
        const string expectedDescription = "newlyAddedDescription";

        var metadata = new OgcStyleMetadata
        {
            Id = styleId,
            Title = expectedTitle,
            Description = expectedDescription,
            Created = DateTime.Today,
            Updated = DateTime.Today,
            PointOfContact = "test"
        };
        await _metadataStorage.Add(collectionId, styleId, metadata);

        var metadataPath = Path.Combine(_options.BaseDirectory, collectionId, styleId, _options.MetadataFilename);
        Assert.True(File.Exists(metadataPath));

        var metadataContentBeforeAdd = JsonSerializer.Serialize(metadata);
        var metadataContentAfterAdd = await File.ReadAllTextAsync(metadataPath);
        Assert.Equal(metadataContentBeforeAdd, metadataContentAfterAdd);
    }

    [Fact]
    public async Task GetMetadata_ShouldReturnMetadata()
    {
        const string collectionId = FileSystemFixture.CollectionId;
        const string styleId = FileSystemFixture.ExistingStyleId;
        const string expectedTitle = "ExistingStyleTitle";
        const string expectedDescription = "For test purpose only";

        var metadata = await _metadataStorage.Get(collectionId, styleId);

        Assert.NotNull(metadata);
        Assert.Equal(expectedTitle, metadata.Title);
        Assert.Equal(expectedDescription, metadata.Description);
    }
}