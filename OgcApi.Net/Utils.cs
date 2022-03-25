using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Features;
using OgcApi.Net.DataProviders;
using OgcApi.Net.Options.Features;
using System;
using System.Linq;

namespace OgcApi.Net
{
    public static class Utils
    {
        public static Uri GetBaseUrl(HttpRequest request, bool withTrailingSlash = true)
        {
            var forwardedProtocol = request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            var url = $"{forwardedProtocol ?? request.Scheme}://{request.Host}{request.PathBase}/api/ogc";
            if (withTrailingSlash)
                url += "/";
            return new Uri(url);
        }

        public static IFeaturesProvider GetFeaturesProvider(IServiceProvider serviceProvider, string dataProviderType)
        {
            var dataProviders = serviceProvider.GetServices<IFeaturesProvider>();
            foreach (var dataProvider in dataProviders)
            {
                if (dataProvider.SourceType == dataProviderType)
                {
                    return dataProvider;
                }
            }
            throw new InvalidOperationException($"Features provider {dataProviderType} is not registered");
        }

        public static ITilesProvider GetTilesProvider(IServiceProvider serviceProvider, string dataProviderType)
        {
            var dataProviders = serviceProvider.GetServices<ITilesProvider>();
            foreach (var dataProvider in dataProviders)
            {
                if (dataProvider.SourceType == dataProviderType)
                {
                    return dataProvider;
                }
            }
            throw new InvalidOperationException($"Tiles provider {dataProviderType} is not registered");
        }

        public static IFeaturesSourceOptions GetCollectionSourceOptions(IServiceProvider serviceProvider, string collectionId)
        {
            var dataProviders = serviceProvider.GetServices<IFeaturesProvider>();
            foreach (var dataProvider in dataProviders)
            {
                var collectionSourcesOptions = dataProvider.CollectionsOptions;
                var collectionSourceOptions = collectionSourcesOptions.GetSourceById(collectionId);
                if (collectionSourceOptions != null && collectionSourceOptions.Features != null)
                    return collectionSourceOptions.Features.Storage;
            }
            throw new InvalidOperationException($"Collection source with id {collectionId} is not found");
        }

        public static string GetFeatureETag(IFeature feature)
        {
            var featureHashString = feature.Geometry + string.Join(' ', feature.Attributes.GetNames()) + string.Join(' ', feature.Attributes.GetValues());
            return "\"" + featureHashString.GetHashCode() + "\"";
        }
    }
}
