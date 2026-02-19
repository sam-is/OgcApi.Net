using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using NetTopologySuite.Features;
using OgcApi.Net.DataProviders;
using System;
using System.Linq;

namespace OgcApi.Net;

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
            var providerType = dataProvider.GetType();
            if (!Attribute.IsDefined(providerType, typeof(OgcFeaturesProviderAttribute))) continue;

            var attribute =
                (OgcFeaturesProviderAttribute)Attribute.GetCustomAttribute(providerType,
                    typeof(OgcFeaturesProviderAttribute));

            if (attribute != null && attribute.Name == dataProviderType)
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
            var providerType = dataProvider.GetType();
            if (!Attribute.IsDefined(providerType, typeof(OgcTilesProviderAttribute))) continue;

            var attribute =
                (OgcTilesProviderAttribute)Attribute.GetCustomAttribute(providerType,
                    typeof(OgcTilesProviderAttribute));

            if (attribute != null && attribute.Name == dataProviderType)
            {
                return dataProvider;
            }
        }
        throw new InvalidOperationException($"Tiles provider {dataProviderType} is not registered");
    }

    public static string GetFeatureETag(IFeature feature)
    {
        var featureHashString = feature.Geometry + string.Join(' ', feature.Attributes.GetNames()) + string.Join(' ', feature.Attributes.GetValues());
        return "\"" + featureHashString.GetHashCode() + "\"";
    }

    public static OpenApiSpecVersion GetOpenApiSpecVersion(string version) => version switch
    {
        "2.0" => OpenApiSpecVersion.OpenApi2_0,
        "3.0" => OpenApiSpecVersion.OpenApi3_0,
        "3.1" => OpenApiSpecVersion.OpenApi3_1,
        "3.2" => OpenApiSpecVersion.OpenApi3_2,
        _ => throw new NotImplementedException($"OpenApi version {version} is not valid. Valid version values are: 2.0, 3.0, 3.1, 3.2")
    };
}