using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Modules;
using OgcApi.Net.OpenApi.Interfaces;
using OgcApi.Net.Styles.Model.Metadata;
using OgcApi.Net.Styles.Model.Styles;
using OgcApi.Net.Styles.Security;
using OgcApi.Net.Styles.Storage.FileSystem;

namespace OgcApi.Net.Styles.Extensions;

public static class StylesServicesExtensions
{
    public static IServiceCollection AddOgcApiStyles(this IServiceCollection services)
    {
        services.AddSingleton<ILinksExtension, StylesLinksExtension>();
        services.AddSingleton<IOpenApiExtension, StylesOpenApiExtension>();
        return services;
    }

    public static IServiceCollection AddStylesStorage<T>(this IServiceCollection services) where T : class, IStyleStorage
    {
        services.AddSingleton<IStyleStorage, T>();
        return services;
    }

    public static IServiceCollection AddStylesMetadataStorage<T>(this IServiceCollection services) where T : class, IMetadataStorage
    {
        services.AddSingleton<IMetadataStorage, T>();
        return services;
    }

    public static IServiceCollection AddStyleAuthorization<T>(this IServiceCollection services) where T : class, IStylesAuthorizationService
    {
        services.AddSingleton<IStylesAuthorizationService, T>();
        return services;
    } 
}