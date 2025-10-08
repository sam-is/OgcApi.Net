namespace OgcApi.Net.Styles.Security;

/// <summary>
/// Defines a service for authorizing access to style-related resources.
/// Implement this interface to enforce API key or other access control checks.
/// </summary>
public interface IStylesAuthorizationService
{
    /// <summary>
    /// Authorizes the client for accessing styles resources using the specified API key
    /// </summary>
    /// <param name="apiKey">The API key used for authentication</param>
    /// <param name="baseResource">Base resource identifier</param>
    /// <param name="styleId">Style identifier</param>
    /// <returns>No value returned if authorization is successful. If authorization failed then the 
    /// method must throw  UnauthorizedAccessException</returns>
    public Task Authorize(string? apiKey = null, string? baseResource = null, string? styleId = null);
}
