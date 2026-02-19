using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OgcApi.Net.Modules;
using OgcApi.Net.Options;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OgcApi.Net.Controllers;

[EnableCors("OgcApi")]
[ApiController]
[Route("api/ogc/conformance")]
[ApiExplorerSettings(GroupName = "ogc")]
public class ConformanceController : ControllerBase
{
    private readonly OgcApiOptions _apiOptions;

    private readonly ILogger _logger;

    private readonly IEnumerable<ILinksExtension> _linksExtensions;

    public ConformanceController(IOptionsMonitor<OgcApiOptions> apiOptions, ILoggerFactory logger,
        IEnumerable<ILinksExtension> linksExtensions)
    {
        _apiOptions = apiOptions.CurrentValue;
        _linksExtensions = linksExtensions;
        _logger = logger.CreateLogger("OgcApi.Net.Controllers.ConformanceController");

        try
        {
            OgcApiOptionsValidator.Validate(apiOptions.CurrentValue);
        }
        catch (OptionsValidationException ex)
        {
            foreach (var failure in ex.Failures)
            {
                _logger.LogError("{failure}", failure);
            }
            throw;
        }
    }

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<Conformance> Get()
    {
        _logger.LogTrace("Get conformance with parameters {queryString}", Request.QueryString);

        if (_apiOptions?.Conformance?.ConformsTo != null && _apiOptions.Conformance.ConformsTo.Count != 0)
            return Ok(new Conformance
            {
                ConformsTo = _apiOptions.Conformance.ConformsTo
            });

        var conformsTo = new List<Uri>();

        if (_apiOptions != null && _apiOptions.Collections.Items.Any(item => item.Features != null))
        {
            conformsTo.Add(new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core"));
            conformsTo.Add(new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30"));
            conformsTo.Add(new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/html"));
            conformsTo.Add(new Uri("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/geojson"));
            conformsTo.Add(new Uri("http://www.opengis.net/spec/ogcapi-features-2/1.0/conf/crs"));
        }

        if (_apiOptions != null && _apiOptions.Collections.Items.Any(item => item.Tiles != null))
        {
            conformsTo.Add(new Uri("http://www.opengis.net/spec/ogcapi-tiles-1/1.0/conf/tileset"));
            conformsTo.Add(new Uri("http://www.opengis.net/spec/ogcapi-tiles-1/1.0/conf/tilesets-list"));
        }

        foreach (var linkExtension in _linksExtensions)
            conformsTo.AddRange(linkExtension.GetConformsTo());

        return Ok(new Conformance
        {
            ConformsTo = conformsTo
        });
    }
}