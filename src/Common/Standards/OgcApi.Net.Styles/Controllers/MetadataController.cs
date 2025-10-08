using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OgcApi.Net.Styles.Model.Metadata;
using OgcApi.Net.Styles.Security;

namespace OgcApi.Net.Styles.Controllers;

/// <summary>
/// Controller that exposes metadata endpoints for styles.
/// </summary>
[EnableCors("OgcApi")]
[ApiController]
[Route("api/ogc/collections/{collectionId}/styles/{styleId}/metadata")]
[ApiExplorerSettings(GroupName = "ogc")]
public class MetadataController(IMetadataStorage metadataStorage,
    IStylesAuthorizationService? authorizationService = null) : ControllerBase
{
    /// <summary>
    /// Returns metadata for the specified style.
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OgcStyleMetadata>> GetMetadata(string collectionId, string styleId,
        [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId, styleId);

            var metadata = await metadataStorage.Get(collectionId, styleId);
            return Ok(metadata);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Replaces metadata for the specified style with the provided metadata document.
    /// </summary>
    [HttpPut]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ReplaceMetadata(string collectionId, string styleId,
        [FromBody] OgcStyleMetadata newMetadata, [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId, styleId);

            await metadataStorage.Replace(collectionId, styleId, newMetadata);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Updates metadata for the specified style (merge-patch semantics expected by the controller).
    /// </summary>
    [HttpPatch]
    [Consumes("application/merge-patch+json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateMetadata(string collectionId, string styleId,
        [FromBody] OgcStyleMetadata metadata, [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId, styleId);

            await metadataStorage.Update(collectionId, styleId, metadata);
            return Ok();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [NonAction]
    private async Task Authorize(string? apiKey = null, string? collectionId = null,
        string? styleId = null)
    {
        if (authorizationService != null)
            await authorizationService.Authorize(apiKey, collectionId, styleId);    
    }
}