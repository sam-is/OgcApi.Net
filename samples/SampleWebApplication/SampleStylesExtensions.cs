using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OgcApi.Net.Styles.Extensions;
using OgcApi.Net.Styles.Options;
using OgcApi.Net.Styles.Storage.FileSystem;
using SampleWebApplication.Security;

namespace SampleWebApplication;

public static class SampleStylesExtensions
{
    public static IServiceCollection AddSampleOgcStyles(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOgcApiStyles();
        services.Configure<OgcApiStylesOptions>(configuration.GetSection(nameof(OgcApiStylesOptions)));
        services.Configure<StyleFileSystemStorageOptions>(configuration.GetSection(nameof(StyleFileSystemStorageOptions)));
        services.AddStylesStorage<StyleFileSystemStorage>();
        services.AddStylesMetadataStorage<StyleMetadataFileSystemStorage>();
        services.AddStyleAuthorization<SampleStylesAuthorizationService>();
        return services;
    }
}
