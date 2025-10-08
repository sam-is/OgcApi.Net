using Microsoft.Extensions.Options;
using OgcApi.Net.Resources;
using OgcApi.Net.Styles.Model.Metadata;
using OgcApi.Net.Styles.Model.Styles;
using OgcApi.Net.Styles.Model.Stylesheets;
using System.Collections.Concurrent;
using System.Text.Json;

namespace OgcApi.Net.Styles.Storage.FileSystem;

/// <summary>
/// Filesystem-backed implementation of <see cref="IStyleStorage"/>.
/// Manages stylesheet files and the default style file in configured directories.
/// </summary>
public class StyleFileSystemStorage(IOptionsMonitor<StyleFileSystemStorageOptions> styleFileSystemOptions) : IStyleStorage
{
    private static readonly ConcurrentDictionary<string, object> Locks = new();
    private readonly StyleFileSystemStorageOptions _storageOptions = styleFileSystemOptions.CurrentValue;

    /// <summary>
    /// Checks whether a style directory exists for the given base resource and style id.
    /// </summary>
    public Task<bool> StyleExists(string baseResource, string styleId)
    {
        var styleDirectory = Path.Combine(_storageOptions.BaseDirectory, baseResource, styleId);
        return Task.FromResult(Directory.Exists(styleDirectory));
    }

    /// <summary>
    /// Checks whether a stylesheet with the given format exists for the style.
    /// </summary>
    public Task<bool> StylesheetExists(string baseResource, string styleId, string format)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(format);
        var stylesheetName = $"{_storageOptions.StylesheetFilename}.{format}.{stylesheetExtension}";
        var stylesheetPath = Path.Combine(_storageOptions.BaseDirectory, baseResource, styleId, stylesheetName);

        return Task.FromResult(File.Exists(stylesheetPath));
    }

    /// <summary>
    /// Returns a list of available stylesheet formats for the specified style.
    /// </summary>
    public Task<List<string>> GetAvailableFormats(string baseResource, string styleId)
    {
        var stylesheetsPath = Path.Combine(_storageOptions.BaseDirectory, baseResource, styleId);

        if (!Directory.Exists(stylesheetsPath))
            return Task.FromResult(new List<string>());

        var stylesheets = Directory.GetFiles(stylesheetsPath);

        // every stylesheet is stored in format "style.[stylesheet format].[extension]"
        // so here we split filename by points and get stylesheet format
        var availableFormats = stylesheets
            .Select(Path.GetFileName)
            .Where(stylesheet => 
                stylesheet != _storageOptions.DefaultStyleFilename &&
                stylesheet != _storageOptions.MetadataFilename)
            .Select(stylesheet =>
                stylesheet!.Split(".")
                .Skip(1)
                .First())
            .ToList();

        return Task.FromResult(availableFormats);
    }

    /// <summary>
    /// Adds a new stylesheet file for the given style and format.
    /// </summary>
    public Task AddStylesheet(string baseResource, StylesheetAddParameters parameters)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(parameters.Format);
        var stylesheetName = $"{_storageOptions.StylesheetFilename}.{parameters.Format}.{stylesheetExtension}";
        var savePath = Path.Combine(_storageOptions.BaseDirectory, baseResource, parameters.StyleId);

        var lockKey = $"{baseResource}_{parameters.StyleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock (lockObj)
            {
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                File.WriteAllText(Path.Combine(savePath, stylesheetName), parameters.Content);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a style directory and all its contents.
    /// </summary>
    public Task DeleteStyle(string baseResource, string styleId)
    {
        var stylePath = Path.Combine(_storageOptions.BaseDirectory, baseResource, styleId);

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock (lockObj)
            {
                if (!Directory.Exists(stylePath))
                    return Task.CompletedTask;

                Directory.Delete(stylePath, true);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Reads style metadata and returns an <see cref="OgcStyle"/> object with links to stylesheets.
    /// </summary>
    public async Task<OgcStyle> GetStyle(string baseResource, string styleId, Uri baseUrl)
    {
        var metadataPath = Path.Combine(
            _storageOptions.BaseDirectory,
            baseResource, styleId,
            _storageOptions.MetadataFilename);
        if (!File.Exists(metadataPath))
            throw new KeyNotFoundException("Style metadata not found");

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            var availableFormats = await GetAvailableFormats(baseResource, styleId);
            var links = availableFormats
                    .Select(format => new Link
                    {
                        Href = new Uri(baseUrl, $"collections/{baseResource}/styles/{styleId}?f={format}"),
                        Rel = "stylesheet",
                        Type = FormatToContentTypeMapper.GetContentTypeForFormat(format)
                    }).ToList();
            lock (lockObj)
            {
                var metadataContent = File.ReadAllText(metadataPath);
                var metadata = JsonSerializer.Deserialize<OgcStyleMetadata>(metadataContent) ??
                    throw new Exception("Style metadata does not exist");

                return new OgcStyle
                {
                    Id = styleId,
                    Title = metadata.Title,
                    Links = links
                };
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }
    }

    /// <summary>
    /// Returns all styles for a base resource and resolves the collection's default style.
    /// </summary>
    public async Task<OgcStyles> GetStyles(string baseResource, Uri baseUrl)
    {
        var baseResourcePath = Path.Combine(_storageOptions.BaseDirectory, baseResource);

        if (!Directory.Exists(baseResourcePath))
            throw new KeyNotFoundException($"Styles for {baseResource} not found");

        var styles = new OgcStyles();
        var stylesDirectories = Directory.GetDirectories(baseResourcePath);
        foreach (var styleDirectory in stylesDirectories)
        {
            var styleId = Path.GetFileNameWithoutExtension(styleDirectory);
            var style = await GetStyle(baseResource, styleId, baseUrl);
            styles.Styles.Add(style);
        }

        var defaultStyleFilePath = Path.Combine(
            _storageOptions.BaseDirectory,
            baseResource,
            _storageOptions.DefaultStyleFilename);

        var lockKey = $"{baseResource}_defaultStyle";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock(lockObj)
            {
                styles.Default = styles.Styles.FirstOrDefault()?.Id;

                if (File.Exists(defaultStyleFilePath))
                {
                    var defaultStyleFileContent = File.ReadAllText(defaultStyleFilePath);
                    var defaultStyle = JsonSerializer.Deserialize<DefaultStyle>(defaultStyleFileContent);
                    styles.Default = defaultStyle?.Default;
                }
                return styles;
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }   
    }

    /// <summary>
    /// Reads and returns the stylesheet content for the given style and format.
    /// </summary>
    public Task<string> GetStylesheet(string baseResource, string styleId, string format)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(format);
        var stylesheetFilename = $"{_storageOptions.StylesheetFilename}.{format}.{stylesheetExtension}";
        var stylesheetPath = Path.Combine(_storageOptions.BaseDirectory, baseResource, styleId, stylesheetFilename);
        if (!File.Exists(stylesheetPath))
            throw new KeyNotFoundException("Stylesheet not found");

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock(lockObj)
            {
                var content = File.ReadAllText(stylesheetPath);
                return Task.FromResult(content);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }
    }

    /// <summary>
    /// Replaces an existing stylesheet with provided content.
    /// </summary>
    public Task ReplaceStyle(string baseResource, string styleId, StylesheetAddParameters stylePostParameters)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(stylePostParameters.Format);
        var stylesheetName = $"{_storageOptions.StylesheetFilename}.{stylePostParameters.Format}.{stylesheetExtension}";
        var path = Path.Combine(_storageOptions.BaseDirectory, baseResource, stylePostParameters.StyleId, stylesheetName);
        if (!File.Exists(path))
            throw new KeyNotFoundException("Stylesheet not found");

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock (lockObj)
            {
                File.WriteAllText(path, stylePostParameters.Content);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the collection default style file with the provided default style value.
    /// </summary>
    public Task UpdateDefaultStyle(string baseResource, DefaultStyle updateDefaultStyleRequest)
    {
        var defaultStyleFilePath = Path.Combine(_storageOptions.BaseDirectory, baseResource);
        var lockKey = $"{baseResource}_defaultStyle";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock (lockObj)
            {
                if (!Directory.Exists(defaultStyleFilePath))
                    Directory.CreateDirectory(defaultStyleFilePath);

                var defaultStyleFileContent = JsonSerializer.Serialize(updateDefaultStyleRequest);
                File.WriteAllText(Path.Combine(
                    defaultStyleFilePath,
                    _storageOptions.DefaultStyleFilename
                    ), defaultStyleFileContent);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }

        return Task.CompletedTask;
    }
}