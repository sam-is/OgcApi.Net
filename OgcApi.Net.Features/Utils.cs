using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Features;
using OgcApi.Net.Features.DataProviders;
using OgcApi.Net.Features.Options.Interfaces;

namespace OgcApi.Net.Features
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

        public static IDataProvider GetDataProvider(IServiceProvider serviceProvider, string dataProviderType)
        {
            var dataProviders = serviceProvider.GetServices<IDataProvider>();
            foreach (var dataProvider in dataProviders)
            {
                if (dataProvider.SourceType == dataProviderType)
                {
                    return dataProvider;
                }
            }
            throw new InvalidOperationException($"Data provider {dataProviderType} is not registered");
        }

        public static ICollectionSourceOptions GetCollectionSourceOptions(IServiceProvider serviceProvider, string collectionId)
        {
            var dataProviders = serviceProvider.GetServices<IDataProvider>();
            foreach (var dataProvider in dataProviders)
            {
                var collectionSourcesOptions = dataProvider.GetCollectionSourcesOptions();
                var collectionSourceOptions = collectionSourcesOptions.GetSourceById(collectionId);
                if (collectionSourceOptions != null)
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
