using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using OgcApi.Net.Features.Crs;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Features;
using OgcApi.Net.Features.Options;
using OgcApi.Net.Features.Resources;
using OgcApi.Net.Features.Temporal;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            Uri baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get collections with parameters {Request.QueryString}");

            var collections = new List<Collection>();
            foreach (CollectionOptions collectionOptions in _apiOptions.Collections.Items)
            {
                collections.Add(GetCollectionMetadata(new Uri(baseUri, $"{collectionOptions.Id}/items"), collectionOptions));
            }

            List<Link> links;
            if (_apiOptions.Collections.Links == null || _apiOptions.Collections.Links.Count == 0)
            {
                links = new List<Link>()
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

            return new Collections()
            {
                Links = links,
                Items = collections
            };
        }

        private Collection GetCollectionMetadata(Uri uri, CollectionOptions collectionOptions)
        {
            IDataProvider dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

            Extent extent = collectionOptions.Extent;
            if (extent == null)
            {
                Envelope envelope = dataProvider.GetBbox(collectionOptions.Id);
                envelope.Transform(collectionOptions.StorageCrs, CrsUtils.DefaultCrs);

                extent = new Extent()
                {
                    Spatial = new SpatialExtent()
                    {
                        Bbox = new[] { new[] { envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY } },
                        Crs = new List<Uri>() { CrsUtils.DefaultCrs }
                    }
                };
            }

            List<Link> links;
            if (_apiOptions.Collections.Links == null || _apiOptions.Collections.Links.Count == 0)
            {
                links = new List<Link>()
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

            var collection = new Collection()
            {
                Id = collectionOptions.Id,
                Title = collectionOptions.Title,
                Description = collectionOptions.Description,
                Extent = extent,
                Links = links,
                Crs = collectionOptions.Crs ?? new List<Uri>() { CrsUtils.DefaultCrs },
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
            Uri baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get collection with parameters {Request.QueryString}");

            CollectionOptions collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                return Ok(GetCollectionMetadata(new Uri(baseUri, "items"), collectionOptions));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{collectionId}/items")]
        [Produces("application/geo+json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            Uri baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get collection items with parameters {Request.QueryString}");

            var validParams = new List<string>() {
                nameof(limit),
                nameof(offset),
                nameof(bbox),
                nameof(bboxCrs),
                nameof(dateTime),
                nameof(crs),
                nameof(apiKey)
            };
            foreach (var param in Request.Query)
            {
                if (!validParams.Contains(param.Key))
                {
                    _logger.LogError($"Unknown parameter {param.Key}");
                    return BadRequest($"Unknown parameter {param.Key}");
                }
            }

            CollectionOptions collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                IDataProvider dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

                if (bboxCrs == null)
                {
                    bboxCrs = CrsUtils.DefaultCrs;
                }

                Envelope envelope = null;
                List<double> bboxCoords = ParseBbox(bbox);
                if (bboxCoords.Count == 4)
                {
                    envelope = new Envelope(bboxCoords[0], bboxCoords[1], bboxCoords[2], bboxCoords[3]);
                    envelope.Transform(bboxCrs, collectionOptions.StorageCrs);
                }
                else
                {
                    _logger.LogError("Invalid bounding box");
                    return BadRequest("Invalid bounding box");
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

                DateTimeInterval dateTimeInterval = DateTimeInterval.Parse(dateTime);

                OgcFeatureCollection features = dataProvider.GetFeatures(
                    collectionOptions.Id,
                    limit,
                    offset,
                    envelope,
                    startDateTime: dateTimeInterval.Start,
                    endDateTime: dateTimeInterval.End,
                    apiKey: apiKey);
                features.Transform(collectionOptions.StorageCrs, crs);

                features.Links = new List<Link>()
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
                    startDateTime: dateTimeInterval.Start,
                    endDateTime: dateTimeInterval.End,
                    apiKey: apiKey);

                if (offset + limit < features.TotalMatched)
                {
                    var parameters = HttpUtility.ParseQueryString(Request.QueryString.ToString());
                    parameters.Set("offset", (offset + limit).ToString());

                    features.Links.Add(new Link()
                    {
                        Href = new Uri(baseUri, $"?{parameters}"),
                        Rel = "next",
                        Type = "application/geo+json"
                    });
                }

                Response.Headers.Add("Content-Crs", crs.ToString());

                return Ok(features);
            }
            else
            {
                _logger.LogError($"Cannot find options for specified collection {collectionId}");
                return NotFound();
            }
        }

        private static List<double> ParseBbox(string bbox)
        {
            var result = new List<double>();
            string[] tokens = bbox.Split(',');
            foreach (string token in tokens)
            {
                if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out double coord))
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
            Uri baseUri = Utils.GetBaseUrl(Request);

            _logger.LogTrace($"Get feature with parameters {Request.QueryString}");

            CollectionOptions collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                IDataProvider dataProvider = Utils.GetDataProvider(_serviceProvider, collectionOptions.SourceType);

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

                OgcFeature feature = dataProvider.GetFeature(collectionOptions.Id, featureId, apiKey: apiKey);
                feature.Transform(collectionOptions.StorageCrs, crs);

                feature.Links = new List<Link>()
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
            else
            {
                _logger.LogError($"Cannot find options for specified collection {collectionId}");
                return NotFound();
            }
        }
    }
}