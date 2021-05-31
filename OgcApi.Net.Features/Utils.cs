using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Features.DataProviders;
using System;
using System.Collections.Generic;

namespace OgcApi.Net.Features
{
    public static class Utils
    {
        public static Uri GetBaseUrl(HttpRequest request)
        {            
            return new($"{request.Scheme}://{request.Host}{request.Path}/");
        }

        public static IDataProvider GetDataProvider(IServiceProvider serviceProvider, string dataProviderType)
        {
            IEnumerable<IDataProvider> dataProviders = serviceProvider.GetServices<IDataProvider>();
            foreach (IDataProvider dataProvider in dataProviders)
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
