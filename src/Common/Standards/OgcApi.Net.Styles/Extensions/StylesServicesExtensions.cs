using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Modules;
using OgcApi.Net.OpenApi.Interfaces;
using OgcApi.Net.Styles.Model.Metadata;
using OgcApi.Net.Styles.Model.Styles;
using OgcApi.Net.Styles.Security;

namespace OgcApi.Net.Styles.Extensions;

/// <summary>
/// Extension methods to register styles features and storage into the DI container.
/// </summary>
public static class StylesServicesExtensions
{
    public static IServiceCollection AddOgcApiStyles(this IServiceCollection services)
    {
        services.AddSingleton<ILinksExtension, StylesLinksExtension>();
        services.AddSingleton<IOpenApiExtension, StylesOpenApiExtension>();
        return services;
    }

    /// <summary>
    /// Registers an <see cref="IStyleStorage"/> implementation as singleton.
    /// </summary>
    public static IServiceCollection AddStylesStorage<T>(this IServiceCollection services) where T : class, IStyleStorage
    {
        services.AddSingleton<IStyleStorage, T>();
        return services;
    }

    /// <summary>
    /// Registers an <see cref="IMetadataStorage"/> implementation as singleton.
    /// </summary>
    public static IServiceCollection AddStylesMetadataStorage<T>(this IServiceCollection services) where T : class, IMetadataStorage
    {
        services.AddSingleton<IMetadataStorage, T>();
        return services;
    }

    /// <summary>
    /// Registers an <see cref="IStylesAuthorizationService"/> implementation as singleton.
    /// </summary>
    public static IServiceCollection AddStyleAuthorization<T>(this IServiceCollection services) where T : class, IStylesAuthorizationService
    {
        services.AddSingleton<IStylesAuthorizationService, T>();
        return services;
    } 
}