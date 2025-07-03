using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OgcApi.Net.Options;
using OgcApi.Net.Resources;
using OgcApi.Net.Schemas.Options;
using OgcApi.Net.Schemas.Options.Validators;
using OgcApi.Net.Schemas.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OgcApi.Net.Schemas.Controllers;

[EnableCors("OgcApi")]
[ApiController]
[Route("api/ogc/collections")]
[ApiExplorerSettings(GroupName = "ogc")]
public class SchemasController : ControllerBase
{
    private readonly OgcApiOptions _apiOptions;
    private readonly ILogger _logger;
    private readonly ISchemaGenerator _schemaGenerator;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public SchemasController(IOptionsMonitor<OgcApiOptions> apiOptions, ISchemaGenerator schemaGenerator, ILoggerFactory logger)
    {
        ArgumentNullException.ThrowIfNull(apiOptions);

        _apiOptions = apiOptions.CurrentValue;
        _schemaGenerator = schemaGenerator;

        _logger = logger.CreateLogger("OgcApi.Net.Controllers.CollectionsController");

        try
        {
            OgcApiOptionsValidator.Validate(_apiOptions);

            foreach (var schemaCollectionOptions in _apiOptions.Collections.Items.Where(i => i is SchemaCollectionOptions).Cast<SchemaCollectionOptions>())
                SchemaCollectionOptionsValidator.Validate(schemaCollectionOptions);
        }
        catch (OptionsValidationException ex)
        {
            foreach (var failure in ex.Failures)
            {
                _logger.LogError(failure);
            }
            throw;
        }
    }

    [HttpGet("{collectionId}/schema")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Collection> GetSchema(string collectionId)
    {
        var baseUri = Utils.GetBaseUrl(Request);

        _logger.LogTrace("Get collection with parameters {query}", Request.QueryString);

        var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
        if (collectionOptions != null)
        {
            var schema = _schemaGenerator.GenerateSchema(baseUri, collectionOptions);

            var json = JsonSerializer.Serialize(schema, _jsonSerializerOptions);
            return Content(json, "application/json");
        }

        return NotFound();
    }

    [HttpGet("{collectionId}/queryables")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Collection> GetQueryables(string collectionId)
    {
        var baseUri = Utils.GetBaseUrl(Request);

        _logger.LogTrace("Get collection with parameters {query}", Request.QueryString);

        var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
        if (collectionOptions != null)
        {
            var schema = _schemaGenerator.GenerateQueryablesSchema(baseUri, collectionOptions);

            var json = JsonSerializer.Serialize(schema, _jsonSerializerOptions);
            return Content(json, "application/json");
        }

        return NotFound();
    }

    [HttpGet("{collectionId}/sortables")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Collection> GetSortables(string collectionId)
    {
        var baseUri = Utils.GetBaseUrl(Request);

        _logger.LogTrace("Get collection with parameters {query}", Request.QueryString);

        var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
        if (collectionOptions != null)
        {
            var schema = _schemaGenerator.GenerateSortablesSchema(baseUri, collectionOptions);

            var json = JsonSerializer.Serialize(schema, _jsonSerializerOptions);
            return Content(json, "application/json");
        }

        return NotFound();
    }
}