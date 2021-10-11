using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features.Crs;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Resources;
using OgcApi.Net.Features.Temporal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace OgcApi.Net.Features.Controllers
{
    [ApiController]
    [Route("api/ogc/collections")]
    [ApiExplorerSettings(GroupName = "ogc")]
    public class CollectionsController : ControllerBase
    {
        private readonly OgcApiOptions _apiOptions;

        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger _logger;

        public CollectionsController(IOptionsMonitor<OgcApiOptions> apiOptions, IServiceProvider serviceProvider, ILoggerFactory logger)
        {
            if (apiOptions == null) throw new ArgumentNullException(nameof(apiOptions));

            _apiOptions = apiOptions.CurrentValue;
            _serviceProvider = serviceProvider;

            _logger = logger.CreateLogger("OGC API Features CollectionsController");

            try
            {
                OgcApiOptionsValidator.Validate(_apiOptions);
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
        public Collections Get()
        {
            var baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get collections with parameters {Request.QueryString}");

            var collections = _apiOptions.Collections.Items
                .Select(collectionOptions => GetCollectionMetadata(new Uri(
                        baseUri,
                        $"collections/{collectionOptions.Id}/items"),
                    collectionOptions))
                .ToList();

            List<Link> links;
            if (_apiOptions.Collections.Links == null || _apiOptions.Collections.Links.Count == 0)
            {
                links = new List<Link>
                {
                    new()
                    {
                        Href = baseUri,
                        Rel = "self",
                        Type = "application/json"
                    },
                    new()
                    {
                        Href = _apiOptions.LandingPage.ApiDescriptionPage,
                        Rel = "alternate",
                        Type = "text/html"
                    }
                };
            }
            else
            {
                links = _apiOptions.Collections.Links;
            }

            return new Collections
            {
                Links = links,
                Items = collections
            };
        }

        private Collection GetCollectionMetadata(Uri uri, CollectionOptions collectionOptions)
        {
            var dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

            var extent = collectionOptions.Extent;
            if (extent == null)
            {
                var envelope = dataProvider.GetBbox(collectionOptions.Id);
                if (envelope != null)
                {
                    envelope.Transform(collectionOptions.StorageCrs, CrsUtils.DefaultCrs);

                    extent = new Extent
                    {
                        Spatial = new SpatialExtent
                        {
                            Bbox = new[] { new[] { envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY } },
                            Crs = new List<Uri> { CrsUtils.DefaultCrs }
                        }
                    };
                }
            }

            List<Link> links;
            if (_apiOptions.Collections.Links == null || _apiOptions.Collections.Links.Count == 0)
            {
                links = new List<Link>
                {
                    new()
                    {
                        Href = uri,
                        Rel = "items",
                        Type = "application/geo+json",
                        Title = collectionOptions.Title
                    },
                    new()
                    {
                        Href = _apiOptions.LandingPage.ApiDescriptionPage,
                        Rel = "alternate",
                        Type = "text/html"
                    }
                };
            }
            else
            {
                links = collectionOptions.Links;
            }

            var collection = new Collection
            {
                Id = collectionOptions.Id,
                Title = collectionOptions.Title,
                Description = collectionOptions.Description,
                Extent = extent,
                Links = links,
                Crs = collectionOptions.Crs ?? new List<Uri> { CrsUtils.DefaultCrs },
                StorageCrs = collectionOptions.StorageCrs ?? CrsUtils.DefaultCrs,
                StorageCrsCoordinateEpoch = collectionOptions.StorageCrsCoordinateEpoch
            };
            return collection;
        }

        [HttpGet("{collectionId}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Collection> Get(string collectionId)
        {
            var baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get collection with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                return Ok(GetCollectionMetadata(new Uri(baseUri, "items"), collectionOptions));
            }

            return NotFound();
        }

        [HttpGet("{collectionId}/items")]
        [Produces("application/geo+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult GetItems(
            string collectionId,
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0,
            [FromQuery] string bbox = null,
            [FromQuery(Name = "bbox-crs")] Uri bboxCrs = null,
            [FromQuery(Name = "datetime")] string dateTime = null,
            [FromQuery] Uri crs = null,
            [FromQuery] string apiKey = null)
        {
            var baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get collection items with parameters {Request.QueryString}");

            var validParams = new List<string>
            {
                nameof(limit),
                nameof(offset),
                nameof(bbox),
                nameof(crs),
                nameof(apiKey),
                "bbox-crs",
                "datetime",
            };
            foreach (var param in Request.Query)
            {
                if (validParams.Contains(param.Key)) continue;
                _logger.LogError($"Unknown parameter {param.Key}");
                return BadRequest($"Unknown parameter {param.Key}");
            }

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                var dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

                if (bboxCrs == null)
                {
                    bboxCrs = CrsUtils.DefaultCrs;
                }

                Envelope envelope = null;
                var bboxCoords = ParseBbox(bbox);
                if (bboxCoords.Count != 0)
                {
                    if (bboxCoords.Count == 4)
                    {
                        envelope = new Envelope(bboxCoords[0], bboxCoords[2], bboxCoords[1], bboxCoords[3]);
                        envelope.Transform(bboxCrs, collectionOptions.StorageCrs);
                    }
                    else
                    {
                        _logger.LogError("Invalid bounding box");
                        return BadRequest("Invalid bounding box");
                    }
                }

                if (crs != null)
                {
                    if (!collectionOptions.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                var dateTimeInterval = DateTimeInterval.Parse(dateTime);

                var features = dataProvider.GetFeatures(
                    collectionOptions.Id,
                    limit,
                    offset,
                    envelope,
                    dateTimeInterval.Start,
                    dateTimeInterval.End,
                    apiKey);
                features.Transform(collectionOptions.StorageCrs, crs);

                features.Links = new List<Link>
                {
                    new()
                    {
                        Href = baseUri,
                        Rel = "self",
                        Type = "application/geo+json"
                    },
                    new()
                    {
                        Href = collectionOptions.FeatureHtmlPage != null ? collectionOptions.FeatureHtmlPage(collectionId) : _apiOptions.LandingPage.ApiDescriptionPage,
                        Rel = "alternate",
                        Type = "text/html"
                    }
                };

                features.TotalMatched = dataProvider.GetFeaturesCount(
                    collectionOptions.Id,
                    envelope,
                    dateTimeInterval.Start,
                    dateTimeInterval.End,
                    apiKey);

                if (offset + limit < features.TotalMatched)
                {
                    var parameters = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                    parameters.Set("offset", (offset + limit).ToString());

                    features.Links.Add(new Link
                    {
                        Href = new Uri(baseUri, $"?{parameters}"),
                        Rel = "next",
                        Type = "application/geo+json"
                    });
                }

                Response.Headers.Add("Content-Crs", crs.ToString());

                return Ok(features);
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }

        private static List<double> ParseBbox(string bbox)
        {
            var result = new List<double>();
            if (string.IsNullOrWhiteSpace(bbox)) return result;
            var tokens = bbox.Split(',');
            foreach (var token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var coord))
                {
                    result.Add(coord);
                }
            }
            return result;
        }

        [HttpGet("{collectionId}/items/{featureId}")]
        [Produces("application/geo+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetFeature(
            string collectionId,
            string featureId,
            [FromQuery] Uri crs = null,
            [FromQuery] string apiKey = null)
        {
            var baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get feature with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                var dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

                if (crs != null)
                {
                    if (!collectionOptions.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                var feature = dataProvider.GetFeature(collectionOptions.Id, featureId, apiKey);

                Response.Headers.Add("ETag", Utils.GetFeatureETag(feature));

                if (feature == null)
                {
                    return NotFound();
                }

                feature.Transform(collectionOptions.StorageCrs, crs);

                feature.Links = new List<Link>
                {
                    new()
                    {
                        Href = new Uri(baseUri, $"{collectionOptions.Id}/items/{featureId}"),
                        Rel = "self",
                        Type = "application/geo+json"
                    },
                    new()
                    {
                        Href = collectionOptions.FeatureHtmlPage != null ? collectionOptions.FeatureHtmlPage(collectionId) : _apiOptions.LandingPage.ApiDescriptionPage,
                        Rel = "alternate",
                        Type = "text/html"
                    },
                    new()
                    {
                        Href = new Uri(baseUri, $"{collectionOptions.Id}/items"),
                        Rel = "collection",
                        Type = "application/json"
                    }
                };

                return Ok(feature);
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }

        [HttpPost("{collectionId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult CreateFeature(
            string collectionId,
            [FromBody] IFeature feature,
            [FromQuery] Uri crs = null,
            [FromQuery] string apiKey = null)
        {
            var baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Create feature with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                if (!collectionOptions.AllowCreate)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

                if (crs != null)
                {
                    if (!collectionOptions.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                feature.Transform(crs, collectionOptions.StorageCrs);
                feature.Geometry.SRID = int.Parse(collectionOptions.StorageCrs.Segments.Last());

                var createdFeatureId = dataProvider.CreateFeature(collectionId, feature, apiKey);
                return Created($"{baseUri}/{collectionId}/items/{createdFeatureId}", createdFeatureId);
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }

        [HttpPut("{collectionId}/items/{featureId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public ActionResult ReplaceFeature(
            string collectionId,
            string featureId,
            [FromBody] IFeature feature,
            [FromQuery] Uri crs = null,
            [FromQuery] string apiKey = null)
        {
            _logger.LogTrace($"Replace feature with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                if (!collectionOptions.AllowReplace)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

                if (crs != null)
                {
                    if (!collectionOptions.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                feature.Transform(crs, collectionOptions.StorageCrs);
                feature.Geometry.SRID = int.Parse(collectionOptions.StorageCrs.Segments.Last());

                if (Request.Headers.ContainsKey("If-Match"))
                {
                    var requestETag = Request.Headers["If-Match"].First();
                    var providerETag = Utils.GetFeatureETag(dataProvider.GetFeature(collectionId, featureId, apiKey));
                    if (requestETag != providerETag)
                    {
                        return Problem(statusCode: 412);
                    }
                }

                dataProvider.ReplaceFeature(collectionId, featureId, feature, apiKey);
                Response.Headers.Add("ETag", Utils.GetFeatureETag(feature));
                return Ok();
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }

        [HttpDelete("{collectionId}/items/{featureId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteFeature(
            string collectionId,
            string featureId,
            [FromQuery] string apiKey = null)
        {
            _logger.LogTrace($"Delete feature with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                if (!collectionOptions.AllowDelete)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

                dataProvider.DeleteFeature(collectionId, featureId, apiKey);
                return Ok();
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }

        [HttpPatch("{collectionId}/items/{featureId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
        public ActionResult UpdateFeature(
            string collectionId,
            string featureId,
            [FromBody] IFeature feature,
            [FromQuery] Uri crs = null,
            [FromQuery] string apiKey = null)
        {
            _logger.LogTrace($"Update feature with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                if (!collectionOptions.AllowUpdate)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

                if (crs != null)
                {
                    if (!collectionOptions.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                feature.Transform(crs, collectionOptions.StorageCrs);
                feature.Geometry.SRID = int.Parse(collectionOptions.StorageCrs.Segments.Last());

                if (Request.Headers.ContainsKey("If-Match"))
                {
                    var requestETag = Request.Headers["If-Match"].First();
                    var providerETag = Utils.GetFeatureETag(dataProvider.GetFeature(collectionId, featureId, apiKey));
                    if (requestETag != providerETag)
                    {
                        return Problem(statusCode: 412);
                    }
                }

                dataProvider.UpdateFeature(collectionId, featureId, feature, apiKey);
                Response.Headers.Add("ETag", Utils.GetFeatureETag(dataProvider.GetFeature(collectionId, featureId, apiKey)));
                return Ok();
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }
    }
}