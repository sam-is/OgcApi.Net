using Microsoft.Extensions.Options;
using OgcApi.Net.Styles.Model.Metadata;
using System.Collections.Concurrent;
using System.Text.Json;

namespace OgcApi.Net.Styles.Storage.FileSystem;

/// <summary>
/// Filesystem-backed implementation of <see cref="IMetadataStorage"/>.
/// Stores and retrieves style metadata files using configured filesystem paths.
/// </summary>
public class StyleMetadataFileSystemStorage(IOptionsMonitor<StyleFileSystemStorageOptions> options) : IMetadataStorage
{
    private static readonly ConcurrentDictionary<string, object> Locks = new();
    private readonly StyleFileSystemStorageOptions _options = options.CurrentValue;

    /// <summary>
    /// Adds metadata for a style. Creates a style folder if it does not exist.
    /// </summary>
    public Task Add(string baseResource, string styleId, OgcStyleMetadata metadata)
    {
        var metadataPath = Path.Combine(_options.BaseDirectory, baseResource, styleId);

        var lockKey = $"{baseResource}_{metadata.Id}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            var metadataFileName = _options.MetadataFilename;
            lock (lockObj)
            {
                if (!Directory.Exists(metadataPath))
                    Directory.CreateDirectory(metadataPath);

                var metadataContent = JsonSerializer.Serialize(metadata);
                File.WriteAllText(Path.Combine(metadataPath, metadataFileName), metadataContent);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Reads and returns metadata for the specified style.
    /// </summary>
    public Task<OgcStyleMetadata> Get(string baseResource, string styleId)
    {
        var metadataPath = Path.Combine(_options.BaseDirectory, baseResource, styleId);
        if (!Directory.Exists(metadataPath))
            throw new KeyNotFoundException("Style not found");

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            var metadataFileName = _options.MetadataFilename;
            lock (lockObj)
            {
                var metadataContent = File.ReadAllText(Path.Combine(metadataPath, metadataFileName));
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

    /// <summary>
    /// Replaces existing metadata with the provided instance.
    /// </summary>
    public Task Replace(string baseResource, string styleId, OgcStyleMetadata newMetadata)
    {
        // In case of filesystem storage just override existing metadata file
        return Add(baseResource, styleId, newMetadata);
    }

    /// <summary>
    /// Updates existing metadata (filesystem implementation simply overwrites the metadata file).
    /// </summary>
    public Task Update(string baseResource, string styleId, OgcStyleMetadata updatedMetadata)
    {
        // In case of filesystem storage just override existing metadata file
        return Add(baseResource, styleId, updatedMetadata);
    }
}