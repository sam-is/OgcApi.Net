using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OgcApi.Net.Crs;
using OgcApi.Net.Options;
using OgcApi.Net.Resources;
using OgcApi.Net.Temporal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OgcApi.Net.Controllers
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
                        Href = Utils.GetBaseUrl(Request, false),
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
            var dataProvider = Utils.GetFeaturesProvider(_serviceProvider, collectionOptions.Features.Storage.Type);

            var extent = collectionOptions.Extent;
            if (extent == null)
            {
                var envelope = dataProvider.GetBbox(collectionOptions.Id);
                if (envelope != null)
                {
                    envelope.Transform(collectionOptions.Features.StorageCrs, CrsUtils.DefaultCrs);

                    extent = new Extent
                    {
                        Spatial = new SpatialExtent
                        {
                            Bbox = new[] { new[] { envelope.MinX, envelope.MinY, envelope.MaxX, envelope.MaxY } },
                            Crs = CrsUtils.DefaultCrs
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
                Crs = collectionOptions.Features.Crs ?? new List<Uri> { CrsUtils.DefaultCrs },
                StorageCrs = collectionOptions.Features.StorageCrs ?? CrsUtils.DefaultCrs,
                StorageCrsCoordinateEpoch = collectionOptions.Features.StorageCrsCoordinateEpoch
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
                return Ok(GetCollectionMetadata(new Uri(baseUri, $"collections/{collectionOptions.Id}/items"), collectionOptions));
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
            foreach (var (key, _) in Request.Query)
            {
                if (validParams.Contains(key)) continue;
                _logger.LogError($"Unknown parameter {key}");
                return BadRequest($"Unknown parameter {key}");
            }

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                var dataProvider = Utils.GetFeaturesProvider(_serviceProvider, collectionOptions.Features.Storage.Type);

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
                        try
                        {
                            envelope.Transform(bboxCrs, collectionOptions.Features.StorageCrs);
                        }
                        catch
                        {
                            // if the coordinate transformation fails, just ignore bbox
                            // this is not correct but is necessary to pass ogc tests
                            // transformation fails in case of WGS84 -> WebMercator at the poles
                            envelope = null;
                        }
                    }
                    else
                    {
                        _logger.LogError("Invalid bounding box");
                        return BadRequest("Invalid bounding box");
                    }
                }

                if (crs != null)
                {
                    if (!collectionOptions.Features.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                if (!collectionOptions.Features.Crs.Contains(bboxCrs))
                {
                    _logger.LogError("Invalid parameter bbox-crs");
                    return BadRequest("Invalid parameter bbox-crs");
                }

                var dateTimeInterval = DateTimeInterval.Parse(dateTime);

                try
                {
                    var features = dataProvider.GetFeatures(
                        collectionOptions.Id,
                        limit,
                        offset,
                        envelope,
                        dateTimeInterval.Start,
                        dateTimeInterval.End,
                        apiKey);
                    features.Transform(collectionOptions.Features.StorageCrs, crs);

                    features.Links = new List<Link>
                    {
                        new()
                        {
                            Href = Utils.GetBaseUrl(Request, false),
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
                            Href = new Uri(baseUri, $"collections/{collectionId}/items?{parameters}"),
                            Rel = "next",
                            Type = "application/geo+json"
                        });
                    }

                    Response.Headers.Add("Content-Crs", $"<{crs}>");

                    return Ok(features);
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                catch (Exception)
                {
                    return Problem();
                }
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
                var dataProvider = Utils.GetFeaturesProvider(_serviceProvider, collectionOptions.Features.Storage.Type);

                if (crs != null)
                {
                    if (!collectionOptions.Features.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                try
                {
                    var feature = dataProvider.GetFeature(collectionOptions.Id, featureId, apiKey);
                    if (feature == null)
                    {
                        return NotFound();
                    }

                    Response.Headers.Add("ETag", Utils.GetFeatureETag(feature));

                    feature.Transform(collectionOptions.Features.StorageCrs, crs);

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

                    Response.Headers.Add("Content-Crs", $"<{crs}>");

                    return Ok(feature);
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                catch (Exception)
                {
                    return Problem();
                }
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
                if (!collectionOptions.Features.Storage.AllowCreate)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetFeaturesProvider(_serviceProvider, collectionOptions.Features.Storage.Type);

                if (crs != null)
                {
                    if (!collectionOptions.Features.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                feature.Transform(crs, collectionOptions.Features.StorageCrs);
                feature.Geometry.SRID = int.Parse(collectionOptions.Features.StorageCrs.Segments.Last());

                try
                {
                    var createdFeatureId = dataProvider.CreateFeature(collectionId, feature, apiKey);
                    return Created($"{baseUri}/{collectionId}/items/{createdFeatureId}", createdFeatureId);
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                catch (Exception)
                {
                    return Problem();
                }
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
                if (!collectionOptions.Features.Storage.AllowReplace)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetFeaturesProvider(_serviceProvider, collectionOptions.Features.Storage.Type);

                if (crs != null)
                {
                    if (!collectionOptions.Features.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                feature.Transform(crs, collectionOptions.Features.StorageCrs);
                feature.Geometry.SRID = int.Parse(collectionOptions.Features.StorageCrs.Segments.Last());

                if (Request.Headers.ContainsKey("If-Match"))
                {
                    var requestETag = Request.Headers["If-Match"].First();
                    var providerETag = Utils.GetFeatureETag(dataProvider.GetFeature(collectionId, featureId, apiKey));
                    if (requestETag != providerETag)
                    {
                        return Problem(statusCode: 412);
                    }
                }

                try
                {
                    dataProvider.ReplaceFeature(collectionId, featureId, feature, apiKey);
                    Response.Headers.Add("ETag", Utils.GetFeatureETag(feature));
                    return Ok();
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                catch (Exception)
                {
                    return Problem();
                }
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
                if (!collectionOptions.Features.Storage.AllowDelete)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetFeaturesProvider(_serviceProvider, collectionOptions.Features.Storage.Type);

                try
                {
                    dataProvider.DeleteFeature(collectionId, featureId, apiKey);
                    return Ok();
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                catch (Exception)
                {
                    return Problem();
                }
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
                if (!collectionOptions.Features.Storage.AllowUpdate)
                {
                    return Unauthorized();
                }

                var dataProvider = Utils.GetFeaturesProvider(_serviceProvider, collectionOptions.Features.Storage.Type);

                if (crs != null)
                {
                    if (!collectionOptions.Features.Crs.Contains(crs))
                    {
                        _logger.LogError("Invalid parameter crs");
                        return BadRequest("Invalid parameter crs");
                    }
                }
                else
                {
                    crs = CrsUtils.DefaultCrs;
                }

                feature.Transform(crs, collectionOptions.Features.StorageCrs);
                feature.Geometry.SRID = int.Parse(collectionOptions.Features.StorageCrs.Segments.Last());

                if (Request.Headers.ContainsKey("If-Match"))
                {
                    var requestETag = Request.Headers["If-Match"].First();
                    var providerETag = Utils.GetFeatureETag(dataProvider.GetFeature(collectionId, featureId, apiKey));
                    if (requestETag != providerETag)
                    {
                        return Problem(statusCode: 412);
                    }
                }

                try
                {
                    dataProvider.UpdateFeature(collectionId, featureId, feature, apiKey);
                    Response.Headers.Add("ETag", Utils.GetFeatureETag(dataProvider.GetFeature(collectionId, featureId, apiKey)));
                    return Ok();
                }
                catch (UnauthorizedAccessException)
                {
                    return Unauthorized();
                }
                catch (Exception)
                {
                    return Problem();
                }
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }

        [HttpGet("{collectionId}/tiles")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetTileSets(string collectionId)
        {
            _logger.LogTrace($"Get collection tileset list with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                var dataProvider = Utils.GetTilesProvider(_serviceProvider, collectionOptions.Tiles.Storage.Type);

                return Ok(new TileSets
                {
                    Items = new List<TileSet>
                    {
                        new TileSet
                        {
                            Title = collectionOptions.Title,
                            Crs = collectionOptions.Tiles.Crs,
                            TileMatrixSetURI = collectionOptions.Tiles.TileMatrixSet,
                            DataType = "vector",
                            TileMatrixSetLimits = dataProvider.GetLimits(collectionId),

                            Links = new List<Link>
                            {
                                new()
                                {
                                    Href = Utils.GetBaseUrl(Request, false),
                                    Rel = "self",
                                    Type = "application/json"
                                },
                                new()
                                {
                                    Href = collectionOptions.Tiles.TileMatrixSet,
                                    Rel = "http://www.opengis.net/def/rel/ogc/1.0/tiling-scheme",
                                    Type = "application/json"
                                }
                            }
                        }
                    }
                });
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }

        [HttpGet("{collectionId}/tiles/{tileMatrix:int}/{tileRow:int}/{tileCol:int}")]
        [Produces("application/vnd.mapbox-vector-tile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetTile(string collectionId, int tileMatrix, int tileRow, int tileCol)
        {
            _logger.LogTrace($"Get collection tile with parameters {Request.QueryString}");

            var collectionOptions = _apiOptions.Collections.Items.Find(x => x.Id == collectionId);
            if (collectionOptions != null)
            {
                var dataProvider = Utils.GetTilesProvider(_serviceProvider, collectionOptions.Tiles.Storage.Type);
                var tileContent = await dataProvider.GetTileAsync(collectionId, tileMatrix, tileRow, tileCol);
                if (tileContent != null)
                {
                    return File(tileContent,
                        "application/vnd.mapbox-vector-tile",
                        "tile.mvt");
                }

                return NoContent();
            }

            _logger.LogError($"Cannot find options for specified collection {collectionId}");
            return NotFound();
        }
    }
}
