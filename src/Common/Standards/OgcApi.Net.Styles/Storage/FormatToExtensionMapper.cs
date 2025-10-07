namespace OgcApi.Net.Styles.Storage;

/// <summary>
/// Maps stylesheet format names (e.g. "mapbox", "sld10") to file extensions.
/// </summary>
public static class FormatToExtensionMapper
{
    private static readonly Dictionary<string, string> Mappings = new() {
        { "mapbox", "json" },
        { "sld10", "xml" },
        { "sld11", "xml" }
    };

    /// <summary>
    /// Returns the file extension for the provided stylesheet format.
    /// </summary>
    /// <param name="format">A format name such as "mapbox" or "sld10".</param>
    /// <returns>The file extension without leading dot (for example, "json" or "xml").</returns>
    /// <exception cref="System.Exception">Thrown when the format mapping is not found.</exception>
    public static string GetFileExtensionForFormat(string format)
    {
        var isExtensionPresent = Mappings.TryGetValue(format, out var extension);
        if (!isExtensionPresent || extension is null)
            throw new Exception($"Not found file extension for file format {format}");

        return extension;
    }
}