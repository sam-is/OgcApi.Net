using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using OgcApi.Net.OpenApi;
using OgcApi.Net.Options;
using OgcApi.Net.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace OgcApi.Net.Controllers;

[EnableCors("OgcApi")]
[ApiController]
[Route("api/ogc")]
[ApiExplorerSettings(GroupName = "ogc")]
public class LandingPageController : ControllerBase
{
    private readonly OgcApiOptions _apiOptions;

    private readonly ILogger _logger;

    private readonly IOpenApiGenerator _openApiGenerator;

    public LandingPageController(IOptionsMonitor<OgcApiOptions> apiOptions, IOpenApiGenerator openApiGenerator, ILoggerFactory logger)
    {
        _apiOptions = apiOptions.CurrentValue;
        _openApiGenerator = openApiGenerator;

        _logger = logger.CreateLogger("OgcApi.Net.Controllers.LandingPageController");

        try
        {
            OgcApiOptionsValidator.Validate(apiOptions.CurrentValue);
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

    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public LandingPage Get()
    {
        var baseUri = Utils.GetBaseUrl(Request);

        _logger.LogTrace($"Get landing page with parameters {Request.QueryString}");

        List<Link> links;
        if (_apiOptions.LandingPage.Links == null ||
            _apiOptions.LandingPage.Links.Count == 0)
        {
            links =
            [
                new Link
                {
                    Href = Utils.GetBaseUrl(Request, false),
                    HrefLang = "en",
                    Rel = "self",
                    Type = "application/json",
                    Title = "The landing page"
                },
                
                new Link
                {
                    Href = _apiOptions.LandingPage.ApiDescriptionPage,
                    HrefLang = "en",
                    Rel = "service-desc",
                    Type = "application/vnd.oai.openapi+json;version=3.0",
                    Title = "OGC API definition"
                },

                new Link
                {
                    Href = _apiOptions.LandingPage.ApiDocumentPage,
                    HrefLang = "en",
                    Rel = "service-doc",
                    Type = "text/html",
                    Title = "OGC API documentation"
                },

                new Link
                {
                    Href = new Uri(baseUri, "conformance"),
                    HrefLang = "en",
                    Rel = "conformance",
                    Type = "application/json",
                    Title =
                        "Conformance classes from standards or community specifications, identified by a URI, that the API conforms to"
                },

                new Link
                {
                    Href = new Uri(baseUri, "collections"),
                    HrefLang = "en",
                    Rel = "data",
                    Type = "application/json",
                    Title = "Feature collections provided by the API"
                }
            ];
        }
        else
        {
            links = _apiOptions.LandingPage.Links;
        }

        return new LandingPage
        {
            Title = _apiOptions.LandingPage.Title,
            Description = _apiOptions.LandingPage.Description,
            Links = links
        };
    }

    [HttpGet("swagger.json")]
    [Produces("application/vnd.oai.openapi+json;version=3.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult GetOpenApiJson()
    {
        return Content(_openApiGenerator.GetDocument(Utils.GetBaseUrl(Request, false)).SerializeAsJson(OpenApiSpecVersion.OpenApi3_0), "application/vnd.oai.openapi+json;version=3.0",
            Encoding.UTF8);
    }
}