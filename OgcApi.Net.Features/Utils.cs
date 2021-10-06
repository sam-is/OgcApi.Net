using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using System;
using System.Linq;

namespace OgcApi.Net.Features
{
    public static class Utils
    {
        public static Uri GetBaseUrl(HttpRequest request)
        {
            var forwardedProtocol = request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            return new Uri($"{forwardedProtocol ?? request.Scheme}://{request.Host}{request.PathBase}/api/ogc/");
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
    }
}
