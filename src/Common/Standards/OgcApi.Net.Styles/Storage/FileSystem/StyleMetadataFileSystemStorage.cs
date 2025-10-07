using Microsoft.Extensions.Options;
using OgcApi.Net.Styles.Model.Metadata;
using System.Collections.Concurrent;
using System.Text.Json;

namespace OgcApi.Net.Styles.Storage.FileSystem;

public class StyleMetadataFileSystemStorage(IOptionsMonitor<StyleFileSystemStorageOptions> options) : IMetadataStorage
{
    private static readonly ConcurrentDictionary<string, object> Locks = new();
    private readonly StyleFileSystemStorageOptions _options = options.CurrentValue;

    public Task Add(string baseResource, string styleId, OgcStyleMetadata metadata)
    {
        var metadataPath = Path.Combine(_options.BaseDirectory, baseResource, styleId);

        var lockKey = $"{baseResource}_{metadata.Id}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock (lockObj)
            {
                if (!Directory.Exists(metadataPath))
                    Directory.CreateDirectory(metadataPath);

                var metadataContent = JsonSerializer.Serialize(metadata);
                File.WriteAllText(Path.Combine(metadataPath, _options.MetadataFilename), metadataContent);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }

        return Task.CompletedTask;
    }

    public Task<OgcStyleMetadata> Get(string baseResource, string styleId)
    {
        var metadataPath = Path.Combine(_options.BaseDirectory, baseResource, styleId);
        if (!Directory.Exists(metadataPath))
            throw new KeyNotFoundException("Style not found");

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock (lockObj)
            {
                var metadataContent = File.ReadAllText(Path.Combine(metadataPath, _options.MetadataFilename));
                var metadata = JsonSerializer.Deserialize<OgcStyleMetadata>(metadataContent) ??
                    throw new Exception("Failed to deserialize style metadata");
                return Task.FromResult(metadata);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }
    }

    public Task Replace(string baseResource, string styleId, OgcStyleMetadata newMetadata)
    {
        // In case of filesystem storage just override existing metadata file
        return Add(baseResource, styleId, newMetadata);
    }

    public Task Update(string baseResource, string styleId, OgcStyleMetadata updatedMetadata)
    {
        // In case of filesystem storage just override existing metadata file
        return Add(baseResource, styleId, updatedMetadata);
    }
}