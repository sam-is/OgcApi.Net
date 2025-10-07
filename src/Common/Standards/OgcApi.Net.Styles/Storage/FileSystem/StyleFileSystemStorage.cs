using Microsoft.Extensions.Options;
using OgcApi.Net.Resources;
using OgcApi.Net.Styles.Model.Metadata;
using OgcApi.Net.Styles.Model.Styles;
using OgcApi.Net.Styles.Model.Stylesheets;
using System.Collections.Concurrent;
using System.Text.Json;

namespace OgcApi.Net.Styles.Storage.FileSystem;

public class StyleFileSystemStorage(IOptionsMonitor<StyleFileSystemStorageOptions> options) : IStyleStorage
{
    private static readonly ConcurrentDictionary<string, object> Locks = new();
    private readonly StyleFileSystemStorageOptions _options = options.CurrentValue;

    public Task<bool> StyleExists(string baseResource, string styleId)
    {
        var styleDirectory = Path.Combine(_options.BaseDirectory, baseResource, styleId);
        return Task.FromResult(Directory.Exists(styleDirectory));
    }

    public Task<bool> StylesheetExists(string baseResource, string styleId, string format)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(format);
        var stylesheetName = $"{_options.StylesheetFilename}.{format}.{stylesheetExtension}";
        var stylesheetPath = Path.Combine(_options.BaseDirectory, baseResource, styleId, stylesheetName);

        return Task.FromResult(File.Exists(stylesheetPath));
    }

    public Task<List<string>> GetAvailableFormats(string baseResource, string styleId)
    {
        var stylesheetsPath = Path.Combine(_options.BaseDirectory, baseResource, styleId);

        if (!Directory.Exists(stylesheetsPath))
            return Task.FromResult(new List<string>());

        var stylesheets = Directory.GetFiles(stylesheetsPath);

        // every stylesheet is stored in format "style.[stylesheet format].[extension]"
        // so here we split filename by points and get stylesheet format
        var availableFormats = stylesheets
            .Select(Path.GetFileName)
            .Where(stylesheet => stylesheet != _options.DefaultStyleFilename && stylesheet != _options.MetadataFilename)
            .Select(stylesheet =>
                stylesheet!.Split(".")
                .Skip(1)
                .First())
            .ToList();

        return Task.FromResult(availableFormats);
    }

    public Task AddStylesheet(string baseResource, StylesheetAddParameters parameters)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(parameters.Format);
        var stylesheetName = $"{_options.StylesheetFilename}.{parameters.Format}.{stylesheetExtension}";
        var savePath = Path.Combine(_options.BaseDirectory, baseResource, parameters.StyleId);

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

    public Task DeleteStyle(string baseResource, string styleId)
    {
        var stylePath = Path.Combine(_options.BaseDirectory, baseResource, styleId);

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

    public async Task<OgcStyle> GetStyle(string baseResource, string styleId, Uri baseUrl)
    {
        var metadataPath = Path.Combine(_options.BaseDirectory, baseResource, styleId, _options.MetadataFilename);
        if (!File.Exists(metadataPath))
            throw new KeyNotFoundException("Style metadata not found");

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            var metadataContent = await File.ReadAllTextAsync(metadataPath);
            var metadata = JsonSerializer.Deserialize<OgcStyleMetadata>(metadataContent) ??
                throw new Exception("Style metadata does not exist");

            var availableFormats = await GetAvailableFormats(baseResource, styleId);
            var links = availableFormats
                .Select(format => new Link
                {
                    Href = new Uri(baseUrl, $"collections/{baseResource}/styles/{styleId}?f={format}"),
                    Rel = "stylesheet",
                    Type = FormatToContentType.GetContentTypeForFormat(format)
                }).ToList();

            return new OgcStyle
            {
                Id = styleId,
                Title = metadata.Title,
                Links = links
            };
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }
    }

    public async Task<OgcStyles> GetStyles(string baseResource, Uri baseUrl)
    {
        var baseResourcePath = Path.Combine(_options.BaseDirectory, baseResource);

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

        var defaultStyleFilePath = Path.Combine(_options.BaseDirectory, baseResource, _options.DefaultStyleFilename);
        DefaultStyle? defaultStyle;
        var lockKey = $"{baseResource}_defaultStyle";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            if (!File.Exists(defaultStyleFilePath))
            {
                defaultStyle = new DefaultStyle
                {
                    Default = styles.Styles.FirstOrDefault()?.Id
                };
            }
            else
            {
                var defaultStyleFileContent = await File.ReadAllTextAsync(defaultStyleFilePath);
                defaultStyle = JsonSerializer.Deserialize<DefaultStyle>(defaultStyleFileContent);
            }

            styles.Default = defaultStyle?.Default;
            return styles;
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }   
    }

    public async Task<string> GetStylesheet(string baseResource, string styleId, string format)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(format);
        var stylesheetFilename = $"{_options.StylesheetFilename}.{format}.{stylesheetExtension}";
        var stylesheetPath = Path.Combine(_options.BaseDirectory, baseResource, styleId, stylesheetFilename);
        if (!File.Exists(stylesheetPath))
            throw new KeyNotFoundException("Stylesheet not found");

        var lockKey = $"{baseResource}_{styleId}";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            var content = await File.ReadAllTextAsync(stylesheetPath);
            return content;
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }
    }

    public Task ReplaceStyle(string baseResource, string styleId, StylesheetAddParameters stylePostParameters)
    {
        var stylesheetExtension = FormatToExtensionMapper.GetFileExtensionForFormat(stylePostParameters.Format);
        var stylesheetName = $"{_options.StylesheetFilename}.{stylePostParameters.Format}.{stylesheetExtension}";
        var path = Path.Combine(_options.BaseDirectory, baseResource, stylePostParameters.StyleId, stylesheetName);
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

    public Task UpdateDefaultStyle(string baseResource, DefaultStyle updateDefaultStyleRequest)
    {
        var defaultStyleFilePath = Path.Combine(_options.BaseDirectory, baseResource);
        var lockKey = $"{baseResource}_defaultStyle";
        var lockObj = Locks.GetOrAdd(lockKey, _ => new object());
        try
        {
            lock (lockObj)
            {
                if (!Directory.Exists(defaultStyleFilePath))
                    Directory.CreateDirectory(defaultStyleFilePath);

                var defaultStyleFileContent = JsonSerializer.Serialize(updateDefaultStyleRequest);
                File.WriteAllText(Path.Combine(defaultStyleFilePath, _options.DefaultStyleFilename), defaultStyleFileContent);
            }
        }
        finally
        {
            Locks.TryRemove(lockKey, out _);
        }

        return Task.CompletedTask;
    }
}