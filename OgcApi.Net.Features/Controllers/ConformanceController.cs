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
    [Route("api/ogc/conformance")]
    [ApiExplorerSettings(GroupName = "ogc")]
    public class ConformanceController : ControllerBase
    {
        private readonly ConformanceOptions _apiOptions;

        private readonly ILogger _logger;

        public ConformanceController(IOptionsMonitor<OgcApiOptions> apiOptions, ILoggerFactory logger)
        {
            _apiOptions = apiOptions.CurrentValue.Conformance;

            _logger = logger.CreateLogger("OGC API Features ConformanceController");

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
        public Conformance Get()
        {
            _logger.LogTrace($"Get conformance with parameters {Request.QueryString}");

            if (_apiOptions?.ConformsTo == null || _apiOptions.ConformsTo.Count == 0)
            {
                return new Conformance
                {
                    ConformsTo = new List<Uri>
                    {
                        new("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/core"),
                        new("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/oas30"),
                        new("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/html"),
                        new("http://www.opengis.net/spec/ogcapi-features-1/1.0/conf/geojson"),
                        new("http://www.opengis.net/spec/ogcapi-features-2/1.0/conf/crs")
                    }
                };
            }

            return new Conformance
            {
                ConformsTo = _apiOptions.ConformsTo
            };
        }
    }
}
