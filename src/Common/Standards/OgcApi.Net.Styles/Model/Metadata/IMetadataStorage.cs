namespace OgcApi.Net.Styles.Model.Metadata;

/// <summary>
/// Storage interface for style metadata.
/// Implement this to provide custom metadata persistence (DB, object storage, etc.).
/// </summary>
public interface IMetadataStorage
{
    /// <summary>
    /// Adds a metadata of the style
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <param name="metadata">Metadata</param>
    public Task Add(string baseResource, string styleId, OgcStyleMetadata metadata);

    /// <summary>
    /// Returns metadata of the style
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <returns>Style metadata</returns>
    public Task<OgcStyleMetadata> Get(string baseResource, string styleId);

    /// <summary>
    /// Replaces existing style metadata with provided metadata
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <param name="newMetadata">New metadata</param>
    public Task Replace(string baseResource, string styleId, OgcStyleMetadata newMetadata);

    /// <summary>
    /// Updates existing metadata with provided metadata
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <param name="updatedMetadata">Actual metadata</param>
    public Task Update(string baseResource, string styleId, OgcStyleMetadata updatedMetadata);
}