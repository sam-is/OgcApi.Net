namespace OgcApi.Net.Styles.Storage;

/// <summary>
/// Maps stylesheet formats to HTTP content types.
/// </summary>
public static class FormatToContentType
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
    /// <exception cref="System.Exception">Thrown when the format mapping is not found.</exception>
    public static string GetContentTypeForFormat(string format)
    {
        var isExtensionPresent = Mappings.TryGetValue(format, out var extension);
        if (!isExtensionPresent || extension is null)
            throw new Exception($"Not found file extension for file format {format}");

        return extension;
    }
}