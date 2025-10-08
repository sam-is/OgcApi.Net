using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OgcApi.Net.Styles.Model.Metadata;
using OgcApi.Net.Styles.Model.Styles;
using OgcApi.Net.Styles.Model.Stylesheets;
using OgcApi.Net.Styles.Security;

namespace OgcApi.Net.Styles.Controllers;

/// <summary>
/// Controller that exposes endpoints to manage and retrieve styles and stylesheets for collections.
/// </summary>
[EnableCors("OgcApi")]
[ApiController]
[Route("api/ogc/collections/{collectionId}/styles")]
[ApiExplorerSettings(GroupName = "ogc")]
public class StylesController(
    IStyleStorage stylesStorage,
    IMetadataStorage metadataStorage,
    IStylesAuthorizationService? authorizationService = null) : ControllerBase
{
    /// <summary>
    /// Returns list of styles for the specified collection and the default style identifier.
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OgcStyles>> GetStyles(string collectionId, [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId);

            var baseUrl = Utils.GetBaseUrl(Request);
            var styles = await stylesStorage.GetStyles(collectionId, baseUrl);
            return Ok(styles);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }   
        catch (KeyNotFoundException)
        {
            return new OgcStyles();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Returns style information (or a stylesheet when format 'f' is provided) for a given style id.
    /// </summary>
    [HttpGet("{styleId}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetStyle(string collectionId, string styleId,
        [FromQuery] string? f = null, [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId, styleId);

            if (string.IsNullOrEmpty(f))
            {
                var baseUrl = Utils.GetBaseUrl(Request);
                var style = await stylesStorage.GetStyle(collectionId, styleId, baseUrl);
                return Ok(style);
            }

            var stylesheet = await stylesStorage.GetStylesheet(collectionId, styleId, f);
            return Ok(stylesheet);
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
    /// Adds a new stylesheet for the specified collection. Creates metadata for the new style.
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> PostStyle(string collectionId,
        [FromBody] StylesheetAddParameters addStyleParameters, [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId, addStyleParameters.StyleId);

            await stylesStorage.GetStylesheet(collectionId,
                addStyleParameters.StyleId, addStyleParameters.Format);
            return Conflict();
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException)
        {
            // Add new stylesheet
            await stylesStorage.AddStylesheet(collectionId, addStyleParameters);

            // Add a metadata for the new style
            var newlyAddedStyleMetadata = new OgcStyleMetadata
            {
                Id = addStyleParameters.StyleId,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow,
            };
            await metadataStorage.Add(collectionId, addStyleParameters.StyleId, newlyAddedStyleMetadata);

            // Return 201 Created
            return CreatedAtAction(
                nameof(GetStyle),
                new { collectionId, styleId = addStyleParameters.StyleId },
                null
            );
        }
    }

    /// <summary>
    /// Updates the default style for the specified collection.
    /// </summary>
    [HttpPatch]
    [Consumes("application/merge-patch+json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateDefaultStyle(string collectionId,
        [FromBody] DefaultStyle newDefaultStyle, [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId);

            await stylesStorage.UpdateDefaultStyle(collectionId, newDefaultStyle);
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

    /// <summary>
    /// Replaces the existing stylesheet content for the given style identifier.
    /// </summary>
    [HttpPut("{styleId}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ReplaceStyle(string collectionId, string styleId,
        [FromBody] StylesheetAddParameters newStylesheet, [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId, styleId);

            await stylesStorage.ReplaceStyle(collectionId, styleId, newStylesheet);
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

    /// <summary>
    /// Deletes a style
    /// </summary>
    [HttpDelete("{styleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteStyle(string collectionId, string styleId,
        [FromQuery] string? apiKey = null)
    {
        try
        {
            await Authorize(apiKey, collectionId, styleId);

            await stylesStorage.DeleteStyle(collectionId, styleId);
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