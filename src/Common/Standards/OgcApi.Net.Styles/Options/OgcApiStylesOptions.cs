namespace OgcApi.Net.Styles.Options;

/// <summary>
/// Represents configuration options for OGC API styles.
/// </summary>
/// <remarks>This class provides settings to control the behavior of OGC API styles, such as whether authorization
/// is required for requests.</remarks>
public class OgcApiStylesOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether authorization is required for requests.
    /// </summary>
    public bool UseAuthorization { get; set; }
}
