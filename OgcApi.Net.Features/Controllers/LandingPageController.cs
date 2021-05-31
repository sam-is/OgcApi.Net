using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Resources;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features.Controllers
{
    [ApiController]
    [Route("api/ogc")]
    [ApiExplorerSettings(GroupName = "ogc")]
    public class LandingPageController : ControllerBase
    {
        readonly LandingPageOptions _apiOptions;

        private readonly ILogger _logger;

        public LandingPageController(IOptionsMonitor<OgcApiOptions> apiOptions, ILoggerFactory logger)
        {
            _apiOptions = apiOptions.CurrentValue.LandingPage;

            _logger = logger.CreateLogger("OGC API Features LandingPageController");

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
            Uri baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get landing page with parameters {Request.QueryString}");

            List<Link> links;
            if (_apiOptions.Links == null || 
                _apiOptions.Links.Count == 0)
            {
                links = new List<Link>()
                {
                    new()
                    {
                        Href = baseUri,
                        HrefLang = "en",
                        Rel = "self",
                        Type = "application/json",
                        Title = "The landing page"
                    },
                    new()
                    {
                        Href = _apiOptions.ApiDescriptionPage,
                        HrefLang = "en",
                        Rel = "service-desc",
                        Type = "application/vnd.oai.openapi+json;version=3.0",
                        Title = "OGC API definition"
                    },
                    new()
                    {
                        Href = _apiOptions.ApiDocumentPage,
                        HrefLang = "en",
                        Rel = "service-doc",
                        Type = "text/html",
                        Title = "OGC API documentation"
                    },
                    new()
                    {
                        Href = new Uri(baseUri, "conformance"),
                        HrefLang = "en",
                        Rel = "conformance",
                        Type = "application/json",
                        Title = "Conformance classes from standards or community specifications, identified by a URI, that the API conforms to"
                    },
                    new()
                    {
                        Href = new Uri(baseUri, "collections"),
                        HrefLang = "en",
                        Rel = "data",
                        Type = "application/json",
                        Title = "Feature collections provided by the API"
                    }
                };
            }
            else
            {
                links = _apiOptions.Links;
            }            

            return new LandingPage()
            {
                Title = _apiOptions.Title,
                Description = _apiOptions.Description,
                Links = links
            };
        }
    }
}
