namespace OgcApi.Net.Styles.Storage;

/// <summary>
/// Maps stylesheet formats to HTTP content types.
/// </summary>
public static class FormatToContentTypeMapper
{
    private static readonly Dictionary<string, string> Mappings = new() {
        { "mapbox", "application/vnd.mapbox.style+json" },
        { "sld10", "application/vnd.ogc.sld+xml" },
        { "sld11", "application/vnd.ogc.sld+xml" }
    };

    /// <summary>
    /// Returns the HTTP content type for the given stylesheet format.
    /// </summary>
    /// <param name="format">Format name, e.g. "mapbox" or "sld10".</param>
    /// <returns>HTTP media type for the format.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the format mapping is not found.</exception>
    public static string GetContentTypeForFormat(string format)
        => Mappings.TryGetValue(format, out var contentType) ?
            contentType :
            throw new KeyNotFoundException($"Content type for file format {format} not found");
}