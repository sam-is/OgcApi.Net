using OgcApi.Net.Styles.Model.Stylesheets;

namespace OgcApi.Net.Styles.Model.Styles;

/// <summary>
/// Interface that defines storage operations for styles and stylesheets.
/// Implement this to back the styles API with a different storage mechanism.
/// </summary>
public interface IStyleStorage
{
    /// <summary>
    /// Gets stylesheet formats (mapbox, sld10, sld11) for the style
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <returns>List of formats</returns>
    public Task<List<string>> GetAvailableFormats(string baseResource, string styleId);

    /// <summary>
    /// Checks if style already exists
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <returns>Whether the style exists</returns>
    public Task<bool> StyleExists(string baseResource, string styleId);

    /// <summary>
    /// Checks if stylesheet of the style already exists
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <param name="format">Stylesheet format e.g. "mapbox", "sld10"</param>
    /// <returns>Whether the stylesheet exists</returns>
    public Task<bool> StylesheetExists(string baseResource, string styleId, string format);

    /// <summary>
    /// Gets a list of available styles for the resource
    /// </summary>
    /// <param name="baseResource">Resource identifier</param>
    /// <param name="baseUrl">Base URL</param>
    /// <returns>OgcStyles object containing available styles</returns>
    public Task<OgcStyles> GetStyles(string baseResource, Uri baseUrl);

    /// <summary>
    /// Get the style information including stylesheet links
    /// </summary>
    /// <param name="baseResource">Resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <param name="baseUrl">Base URL</param>
    /// <returns>OgcStyle object representing style information</returns>
    public Task<OgcStyle> GetStyle(string baseResource, string styleId, Uri baseUrl);

    /// <summary>
    /// Adds a new style to the styles storage
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="parameters">Style addition parameters</param>
    public Task AddStylesheet(string baseResource, StylesheetAddParameters parameters);

    /// <summary>
    /// Gets a stylesheet of the style with required format
    /// </summary>
    /// <param name="baseResource">Resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <param name="format">Required format</param>
    /// <returns>A content of the stylesheet</returns>
    public Task<string> GetStylesheet(string baseResource, string styleId, string format);

    /// <summary>
    /// Updates default style for baseResource
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="updateDefaultStyleRequest">Default style request</param>
    public Task UpdateDefaultStyle(string baseResource, DefaultStyle updateDefaultStyleRequest);

    /// <summary>
    /// Replaces existing stylesheet with a new stylesheet
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <param name="stylePostParameters">Style post parameters</param>
    public Task ReplaceStyle(string baseResource, string styleId, StylesheetAddParameters stylePostParameters);

    /// <summary>
    /// Deletes existing style
    /// </summary>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    public Task DeleteStyle(string baseResource, string styleId);
}